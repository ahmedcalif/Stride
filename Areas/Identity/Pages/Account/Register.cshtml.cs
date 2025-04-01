using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Stride.Data.Services;

namespace Stride.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }
            
            [Display(Name = "Gender")]
            public Gender? UserGender { get; set; }
            
            [Display(Name = "City")]
            public string City { get; set; }
            
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid during registration");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning($"Model error: {error.ErrorMessage}");
                    }
                    return Page();
                }
                
                _logger.LogInformation($"Starting registration for {Input.Username} / {Input.Email}");
                
                var existingUsername = await _userManager.FindByNameAsync(Input.Username);
                if (existingUsername != null)
                {
                    _logger.LogWarning($"Username {Input.Username} already exists");
                    ModelState.AddModelError(string.Empty, "Username already exists.");
                    return Page();
                }
                
                var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
                if (existingEmail != null)
                {
                    _logger.LogWarning($"Email {Input.Email} already exists");
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return Page();
                }
                
                var user = new ApplicationUser
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserGender = Input.UserGender,
                    City = Input.City,
                    PostalCode = Input.PostalCode,
                    EmailConfirmed = true
                };
                
                _logger.LogInformation($"Creating user {user.UserName}");
                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {   
                  return RedirectToAction("ChooseRole", "Role"); 
                }  
                
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"User creation error: {error.Code} - {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during registration: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration. Please try again.");
                return Page();
            }
        }
    }
}