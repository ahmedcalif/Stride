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
    try
    {
        var username = User.Identity.Name;
        
        // Get goals for this user
        var goalsFromRepo = _goalRepository.GetGoalsByUsername(username).ToList();
        
        // Map goals to view model objects
        List<Stride.Data.Models.Goals> goalsList = goalsFromRepo.Select(g => new Stride.Data.Models.Goals
        {
            Id = g.goal_id,
            Title = g.title,
            Description = g.description,
            TargetDate = g.end_date ?? DateTime.Now,
            IsCompleted = g.is_completed,
            Category = g.Category?.name,
            CategoryId = g.category_id,
            Priority = ConvertPriority(g.goal_priority_id),
            Username = username
        }).ToList();
        
        // Create view model with user details from Identity claims
        var viewModel = new DashboardViewModel
        {
            Username = username,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? username,
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

    private Stride.Data.Models.Priority ConvertPriority(int priorityId)
{
    return priorityId switch
    {
        1 => Stride.Data.Models.Priority.Low,
        2 => Stride.Data.Models.Priority.Medium,
        3 => Stride.Data.Models.Priority.High,
        _ => Stride.Data.Models.Priority.Medium
    };
}
}