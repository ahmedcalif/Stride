using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Stride.ViewModels;
using Stride.Data.Models;
using Stride.Data.Services;

namespace Stride.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUserRepository userRepository,
            ILogger<RoleController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ChooseRole()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                _logger.LogWarning("User not found in Identity database during role selection");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var userRoles = await _userManager.GetRolesAsync(identityUser);
            
            if (userRoles.Count == 0)
            {
                _logger.LogInformation($"User {identityUser.UserName} has no roles. Checking if 'User' role exists.");
                
                bool roleExists = await _roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    _logger.LogInformation("Creating 'User' role as it doesn't exist");
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                
                await _userManager.AddToRoleAsync(identityUser, "User");
                userRoles = new List<string> { "User" };
                
                _logger.LogInformation($"Assigned 'User' role to {identityUser.UserName}");
            }
            
            // Remove this condition to always show the role selection page
            // if (userRoles.Count == 1)
            // {
            //     await SetActiveRole(userRoles.First());
            //     return RedirectToAction("Index", "Dashboard");
            // }
            
            await EnsureUserInCustomDbAsync(identityUser);
            
            // Ensure basic roles exist
            await EnsureBasicRolesExist();
            
            // Get all roles instead of just the user's roles
            var allRoles = await _roleManager.Roles.ToListAsync();
            
            var model = new ChooseRoleViewModel
            {
                AvailableRoles = allRoles
                    .Where(r => userRoles.Contains(r.Name)) // Only show roles the user has
                    .Select(r => new RoleOption
                    {
                        RoleName = r.Name,
                        DisplayName = GetDisplayName(r.Name)
                    }).ToList()
            };

            return View(model);
        }

        private async Task EnsureBasicRolesExist()
        {
            string[] basicRoles = { "User", "Admin" };
            
            foreach (var role in basicRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChooseRole(ChooseRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in ChooseRole post");
                return View(model);
            }
            
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                _logger.LogWarning("User not found during role selection (POST)");
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            var userRoles = await _userManager.GetRolesAsync(identityUser);
            
            if (userRoles.Contains(model.SelectedRole))
            {
                await SetActiveRole(model.SelectedRole);
                
                _logger.LogInformation($"User {identityUser.UserName} selected role: {model.SelectedRole}");
                return RedirectToAction("Index", "Dashboard");
            }
            
            _logger.LogWarning($"Invalid role selection: {model.SelectedRole}");
            ModelState.AddModelError("", "Invalid role selection");
            
            model.AvailableRoles = userRoles.Select(r => new RoleOption
            {
                RoleName = r,
                DisplayName = GetDisplayName(r)
            }).ToList();
            
            return View(model);
        }

        private async Task SetActiveRole(string roleName)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            
            await _signInManager.SignOutAsync();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, roleName),
                new Claim("ActiveRole", roleName)
            };
            
            await _signInManager.SignInWithClaimsAsync(identityUser, isPersistent: true, claims);
            
            _logger.LogInformation($"Set active role '{roleName}' for user {identityUser.UserName}");
        }

        private string GetDisplayName(string roleName)
        {
            return roleName switch
            {
                "Admin" => "Administrator",
                "Manager" => "Department Manager",
                "Editor" => "Content Editor",
                "User" => "Standard User",
                _ => roleName
            };
        }
        
        private async Task EnsureUserInCustomDbAsync(ApplicationUser identityUser)
        {
            try
            {
                var username = identityUser.UserName;
                var customUser = await _userRepository.GetUserByUsername(username);
                
                if (customUser == null)
                {
                    _logger.LogInformation($"Creating custom user record for {username}");
                    var newUser = new User
                    {
                        Username = username,
                        Email = identityUser.Email,
                        Password = "IDENTITY_MANAGED"
                    };
                    
                    _userRepository.CreateUser(newUser);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring user in custom database");
            }
        }
    }
}