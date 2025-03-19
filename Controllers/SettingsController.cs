using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Stride.Data.Models;
using Stride.ViewModels;

namespace Stride.Controllers;

[Route("Settings")]
public class SettingsController : Controller 
{
    private readonly ILogger<SettingsController> _logger;
    private readonly IUserRepository _userRepository;
    
    public SettingsController(ILogger<SettingsController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }
    
    [HttpGet]
    [Route("")]  
    [Route("Index")] 
    public IActionResult Index(int userId = 0)
    {
       
        if (userId <= 0)
        {
            userId = 1; 
        }
        
        try 
        {
            var user = _userRepository.GetUserById(userId);
            
            var viewModel = new SettingsViewModel(user);
            return View(viewModel);
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "User not found with ID {UserId}", userId);
            return NotFound($"User with ID {userId} not found");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving user settings for ID {UserId}", userId);
            return View("Error");
        }
    }
    
    [HttpPost]
    [Route("")] 
    [Route("Index")]
    public IActionResult Index(SettingsViewModel model)
    {
        try 
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }
            
            var user = _userRepository.GetUserById(model.Id);
            model.UpdateUser(user);
            _userRepository.UpdateUser(user);
            
            TempData["SuccessMessage"] = "Settings updated successfully.";
            return RedirectToAction("Index", "Dashboard");
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, "User not found with ID {UserId}", model.Id);
            return NotFound($"User with ID {model.Id} not found");
        }
        catch (Exception e) 
        {
            _logger.LogError(e, "Error updating user settings for ID {UserId}", model.Id);
            ModelState.AddModelError("", "An error occurred while saving settings.");
            return View(model);
        }
    }
}