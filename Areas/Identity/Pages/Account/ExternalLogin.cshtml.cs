using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Stride.Data.Services;

namespace Stride.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

       public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
{
    returnUrl = Url.Content("~/Role/ChooseRole");  // Change this to redirect to role selection
    if (remoteError != null)
    {
        ErrorMessage = $"Error from external provider: {remoteError}";
        return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
    }
    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        ErrorMessage = "Error loading external login information.";
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
    }

    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
    if (result.Succeeded)
    {
        Console.WriteLine($"{info.Principal.Identity.Name} logged in with {info.LoginProvider} provider.");
        
        // Get the current user to check/assign roles
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            
            // If user doesn't have at least 2 roles, add User and Admin roles
            if (userRoles.Count < 2)
            {
                // Make sure the user has both User and Admin roles
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    await EnsureRoleExists("User");
                    await _userManager.AddToRoleAsync(user, "User");
                }
                
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await EnsureRoleExists("Admin");
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                
                // Sign out and sign back in to refresh claims
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
        }
        
        return LocalRedirect(returnUrl);
    }
    if (result.IsLockedOut)
    {
        return RedirectToPage("./Lockout");
    }
    else
    {
        // If the user does not have an account, then ask the user to create an account.
        ReturnUrl = returnUrl;
        ProviderDisplayName = info.ProviderDisplayName;
        if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
        {
            Input = new InputModel
            {
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
            };
        }
        return Page();
    }
}

// Add this helper method to the ExternalLoginModel class
private async Task EnsureRoleExists(string roleName)
{
    var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
{
    returnUrl = Url.Content("~/Role/ChooseRole");  // Change to redirect to role selection
    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        ErrorMessage = "Error loading external login information during confirmation.";
        return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
    }

    if (ModelState.IsValid)
    {
        var user = new ApplicationUser 
        { 
            UserName = Input.Email, 
            Email = Input.Email,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
        };

        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                
                // Ensure roles exist and add user to both roles
                await EnsureRoleExists("User");
                await EnsureRoleExists("Admin");
                
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, "Admin");
                
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                return LocalRedirect(returnUrl);
            }
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    ProviderDisplayName = info.ProviderDisplayName;
    ReturnUrl = returnUrl;
    return Page();
} 
    }
}