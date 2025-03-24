using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Stride.Data.Models;
using Stride.ViewModels;
using Stride.Data.Services;

namespace Stride.Controllers;

[Route("Settings")]
public class SettingsController : Controller 
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    
    public SettingsController(
        ILogger<SettingsController> logger, 
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    [Route("")]  
    [Route("Index")] 
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Account");
        }
        
        try 
        {
            // Get the current user from the Identity system
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }
            
            var appUser = await _userManager.FindByNameAsync(userName);
            
            if (appUser == null)
            {
                _logger.LogWarning($"User with username {userName} not found in identity system");
                
                // Fall back to the repository if Identity fails
                var user = await _userRepository.GetUserByUsername(userName);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var viewModel = new SettingsViewModel(user);
                return View(viewModel);
            }
            
            // For debugging - log the values
            _logger.LogInformation($"Found user: ID={appUser.Id}, Username={appUser.UserName}, Email={appUser.Email}");
            
            var identityViewModel = new SettingsViewModel(appUser);
            return View(identityViewModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving user");
            return RedirectToAction("Index", "Home");
        }
    }
    
    [HttpPost]
    [Route("")] 
    [Route("Index")]
    public async Task<IActionResult> Index(SettingsViewModel model)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Account");
        }
        
        if (!ModelState.IsValid) 
        {
            return View(model);
        }
        
        try 
        {
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }
            
            // Try to get the user from Identity first
            var appUser = await _userManager.FindByNameAsync(userName);
            
            if (appUser != null)
            {
                // Check if username is being changed
                bool usernameChanged = appUser.UserName != model.Username;
                bool emailChanged = appUser.Email != model.Email;
                
                // Update the user with values from the model
                appUser.UserName = model.Username;
                appUser.Email = model.Email;
                appUser.UserGender = model.UserGender;
                appUser.City = model.City;
                appUser.PostalCode = model.PostalCode;
                appUser.TwoFactorEnabled = model.TwoFactorEnabled;
                
                // Handle password change if requested
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    var passwordCheckResult = await _userManager.CheckPasswordAsync(appUser, model.CurrentPassword);
                    if (!passwordCheckResult)
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                        return View(model);
                    }
                    
                    var passwordChangeResult = await _userManager.ChangePasswordAsync(
                        appUser, 
                        model.CurrentPassword, 
                        model.NewPassword
                    );
                    
                    if (!passwordChangeResult.Succeeded)
                    {
                        foreach (var error in passwordChangeResult.Errors)
                        {
                            ModelState.AddModelError("NewPassword", error.Description);
                        }
                        return View(model);
                    }
                }
                
                // Save the changes to the identity user
                var result = await _userManager.UpdateAsync(appUser);
                
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                
                // If username changed, sign the user in again with new username
                if (usernameChanged || emailChanged)
                {
                    await _signInManager.SignInAsync(appUser, isPersistent: false);
                }
                
                TempData["SuccessMessage"] = "Settings updated successfully.";
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                // Fall back to updating with the repository
                var user = await _userRepository.GetUserByUsername(userName);
                
                if (user == null || user.Id != model.Id)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                model.UpdateUser(user);
                _userRepository.UpdateUser(user);
                
                TempData["SuccessMessage"] = "Settings updated successfully.";
                return RedirectToAction("Index", "Dashboard");
            }
        }
        catch (Exception e) 
        {
            _logger.LogError(e, "Error updating user: {Message}", e.Message);
            ModelState.AddModelError("", "An error occurred while saving settings.");
            return View(model);
        }
    }
}