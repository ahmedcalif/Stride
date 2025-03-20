using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stride.Data.Models;
using Stride.ViewModels;
using Stride.Data.DatabaseModels;
using System;
using Microsoft.Extensions.Logging;

namespace Stride.Controllers;
[Authorize]
public class GoalsController : Controller
{
    private readonly IGoalRepository _goalRepository;
    private readonly ILogger<GoalsController> _logger;
    
    public GoalsController(
        IGoalRepository goalRepository,
        ILogger<GoalsController> logger)
    {
        _goalRepository = goalRepository;
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        var username = User.Identity.Name;
        
        var goals = _goalRepository.GetGoalsByUsername(username);
        var viewModel = new GoalListViewModel
        {
            Goals = goals
        };
        return View(viewModel);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        var username = User.Identity.Name;
        
        return View(new GoalViewModel 
        { 
            TargetDate = DateTime.Now.AddDays(30),
            Category = "General" 
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(GoalViewModel model)
    {
        var username = User.Identity.Name;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        try
        {
            var categoryName = string.IsNullOrEmpty(model.Category) ? "General" : model.Category;
            
            var goal = new Goals
            {
                Title = model.Title,
                Description = model.Description,
                TargetDate = model.TargetDate,
                Priority = model.Priority,
                CategoryId = 1, 
                Category = categoryName,
                IsCompleted = model.IsCompleted,
                Username = username
            };
            
            _goalRepository.Add(goal);
            TempData["Success"] = "Goal created successfully!";
            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating goal for user {Username}: {Message}", username, ex.Message);
            ModelState.AddModelError("", "Error creating goal. Please try again.");
            return View(model);
        }
    }
    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var username = User.Identity.Name;
        
        var goal = _goalRepository.GetGoalById(id);
        if (goal == null)
        {
            return NotFound();
        }
        
        if (goal.Username != username)
        {
            return Forbid();
        }
        
        var viewModel = new GoalViewModel
        {
            Id = goal.Id,
            Title = goal.Title,
            Description = goal.Description,
            TargetDate = goal.TargetDate,
            Priority = goal.Priority,
            Category = goal.Category,
            IsCompleted = goal.IsCompleted
        };
        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, GoalViewModel model)
    {
        var username = User.Identity.Name;
        
        if (id != model.Id)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        try
        {
            var existingGoal = _goalRepository.GetGoalById(id);
            if (existingGoal == null)
            {
                return NotFound();
            }
            
            if (existingGoal.Username != username)
            {
                return Forbid();
            }
            
            var categoryName = string.IsNullOrEmpty(model.Category) ? "General" : model.Category;
            
            existingGoal.Title = model.Title;
            existingGoal.Description = model.Description;
            existingGoal.TargetDate = model.TargetDate;
            existingGoal.Priority = model.Priority;
            existingGoal.Category = categoryName;
            existingGoal.IsCompleted = model.IsCompleted;
            
            _goalRepository.Update(existingGoal);
            TempData["Success"] = "Goal updated successfully!";
            return RedirectToAction("Index", "Dashboard"); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating goal for user {Username}: {Message}", username, ex.Message);
            ModelState.AddModelError("", "Error updating goal. Please try again.");
            return View(model);
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var username = User.Identity.Name;
        
        try
        {
            var goal = _goalRepository.GetGoalById(id);
            if (goal == null)
            {
                return NotFound();
            }
            
            if (goal.Username != username)
            {
                return Forbid();
            }
            
            _goalRepository.Delete(id);
            TempData["Success"] = "Goal deleted successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting goal for user {Username}: {Message}", username, ex.Message);
            TempData["Error"] = "Error deleting goal. Please try again.";
        }
        
        return RedirectToAction("Index", "Dashboard"); 
    }
}