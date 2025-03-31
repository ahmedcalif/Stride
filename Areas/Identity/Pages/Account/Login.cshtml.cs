using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Stride.Data.Services;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Stride.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                returnUrl ??= Url.Content("~/Dashboard/Index");
                
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"Login attempt for: {Input.Username}");
                    
                    // Check if input is email
                    bool isEmail = Input.Username.Contains("@");
                    ApplicationUser user = null;
                    
                    // Try to find user by username first
                    user = await _userManager.FindByNameAsync(Input.Username);
                    
                    // If not found and input looks like an email, try by email
                    if (user == null && isEmail)
                    {
                        user = await _userManager.FindByEmailAsync(Input.Username);
                        _logger.LogInformation($"User lookup by email: {(user != null ? "Found" : "Not found")}");
                    }
                    
                    if (user != null)
                    {
                        // Try direct login with username
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        
                        _logger.LogInformation($"Login result: {result.Succeeded}");
                        
                        if (result.Succeeded)
                        {
                            await ProcessRoles(user);
                            return LocalRedirect(returnUrl);
                        }
                        else if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        else if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                    }
                    
                    // If we get here, the login failed
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                
                // If we got this far, something failed, redisplay the form
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during login: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return Page();
            }
        }
        
        private async Task ProcessRoles(ApplicationUser user)
        {
            try
            {
                _logger.LogInformation($"Processing roles for user: {user.UserName}");
                
                var userRoles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation($"User has {userRoles.Count} roles: {string.Join(", ", userRoles)}");
                
                if (userRoles.Count > 1)
                {
                    // If user has multiple roles, redirect to role selector
                    _logger.LogInformation($"User has multiple roles. Redirecting to role selector.");
                }
                else if (userRoles.Count == 1)
                {
                    // If user has exactly one role, sign in with that role
                    await _signInManager.SignOutAsync();
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, userRoles[0]),
                        new Claim("ActiveRole", userRoles[0])
                    };
                    
                    await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, claims);
                    _logger.LogInformation($"Set active role '{userRoles[0]}' for user {user.UserName}");
                }
                else
                {
                    // If user has no roles, assign the default "User" role
                    _logger.LogInformation($"User has no roles. Assigning default 'User' role.");
                    
                    var roleManager = HttpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
                    
                    bool roleExists = await roleManager.RoleExistsAsync("User");
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                        _logger.LogInformation("Created 'User' role");
                    }
                    
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignOutAsync();
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("ActiveRole", "User")
                    };
                    
                    await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, claims);
                    _logger.LogInformation($"Assigned and set active role 'User' for user {user.UserName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing roles: {ex.Message}");
                throw;
            }
        }
    }
}