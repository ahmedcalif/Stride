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

namespace Stride.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
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

       public async Task<IActionResult> OnPostAsync(string returnUrl = null)
{
    returnUrl ??= Url.Content("~/Dashboard/Index");
    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    
    if (ModelState.IsValid)
    {
        var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in.");
            
            var user = await _signInManager.UserManager.FindByNameAsync(Input.Username);
            var userRoles = await _signInManager.UserManager.GetRolesAsync(user);
            
            if (userRoles.Count > 1)
            {
                return RedirectToAction("ChooseRole", "Role");
            }
            else if (userRoles.Count == 1)
            {
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
                var roleManager = HttpContext.RequestServices.GetService<RoleManager<IdentityRole>>();
                
                bool roleExists = await roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }
                
                await _signInManager.UserManager.AddToRoleAsync(user, "User");
                await _signInManager.SignOutAsync();
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("ActiveRole", "User")
                };
                
                await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, claims);
                _logger.LogInformation($"Assigned and set active role 'User' for user {user.UserName}");
            }
            
            return LocalRedirect(returnUrl);
        }
        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToPage("./Lockout");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
    return Page();
} 
    }
}