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
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }
    
    var userRoles = await _userManager.GetRolesAsync(user);
    
    var model = new ChooseRoleViewModel
    {
        AvailableRoles = new List<RoleOption>
        {
            new RoleOption { RoleName = "Admin", DisplayName = "Administrator" },
            new RoleOption { RoleName = "User", DisplayName = "Regular User" }
        },
        SelectedRole = userRoles.FirstOrDefault()
    };
    
    return View(model);
}

[HttpPost]
public async Task<IActionResult> ChooseRole(ChooseRoleViewModel model)
{
    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid model state in ChooseRole post");
        return View(model);
    }
    
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        _logger.LogWarning("User not found during role selection (POST)");
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }
    
    var userRoles = await _userManager.GetRolesAsync(user);
    
    if (userRoles.Contains(model.SelectedRole))
    {
        await SetActiveRole(model.SelectedRole);
    }
    else
    {
        if (userRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, userRoles);
        }
        
        var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
        
        if (!addRoleResult.Succeeded)
        {
            foreach (var error in addRoleResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
        
        await SetActiveRole(model.SelectedRole);
    }
    
    _logger.LogInformation($"User {user.UserName} selected role: {model.SelectedRole}");

    await EnsureUserInCustomDbAsync(user);
    
    return RedirectToAction("Index", "Dashboard");
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