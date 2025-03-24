using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stride.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IGoalRepository _goalRepository;
    
    public DashboardController(
        ILogger<DashboardController> logger,
        IGoalRepository goalRepository, 
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _goalRepository = goalRepository;
    }
  
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
     _logger.LogInformation($"Controller: {this.GetType().Name}, IsAuthenticated: {User.Identity?.IsAuthenticated}, Name: {User.Identity?.Name ?? "null"}");
        foreach (var claim in User.Claims)
        {
    _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
        }
        try
        {
            var username = User.Identity.Name;
            
            var user = await _userRepository.GetUserByUsername(username);
                
          if (user == null)
{
    _logger.LogWarning($"Dashboard access failed - user not found in app database: '{username}'");
    user = new User
    {
        Username = username,
        Email = User.FindFirstValue(ClaimTypes.Email) ?? username,
        Password = "IDENTITY_MANAGED" 
    };
    
    try {
        user = _userRepository.CreateUser(user);
    }
    catch (Exception ex) {
        _logger.LogError(ex, "Failed to create user in custom repository");
    }
} 
            
            List<Goals> goalsList = _goalRepository.GetGoalsByUsername(username).ToList(); 
            var viewModel = new DashboardViewModel
            {
                Username = user.Username,
                Email = user.Email,
                UserGoals = goalsList
            };
            
            _logger.LogInformation($"Dashboard loaded successfully for user: '{username}'");
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accessing dashboard");
            return RedirectToAction("Error", "Home");
        }
    }
    
    [HttpGet]
    public IActionResult Settings()
    {
        var username = User.Identity?.Name;
        return RedirectToAction("Index", "Settings", new { username });
    }
}