using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stride.ViewModels;
using Stride.Data.DatabaseModels;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stride.Data;
using Stride.Data.Data;
using System.Security.Claims;
using Stride.Data.Models;

namespace Stride.Controllers;

[Authorize]
public class GoalsController : Controller
{
    private readonly IGoalRepository _goalRepository;
    private readonly ILogger<GoalsController> _logger;
    private readonly ApplicationDbContext _dbContext;
    
    public GoalsController(
        IGoalRepository goalRepository,
        ILogger<GoalsController> logger,
        ApplicationDbContext dbContext)
    {
        _goalRepository = goalRepository;
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> Index()
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        
        // Get user ID directly from claims
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(identityUserId))
        {
            _logger.LogError("Identity user ID not found in claims for user {Username}", username);
            return RedirectToAction("Error", "Home");
        }
        
        _logger.LogInformation("Looking up goals for identity user ID: {IdentityUserId}", identityUserId);
        
        // Find the user in the custom Users table using the IdentityId from AspNetUsers
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
        
        if (user == null)
        {
            // Create the user in our custom table if they don't exist
            _logger.LogInformation("User {Username} not found in custom table, creating entry", username);
            user = new Stride.Data.DatabaseModels.User
            {
                username = username,
                email = User.FindFirstValue(ClaimTypes.Email) ?? username,
                password_hash = "IDENTITY_MANAGED",
                IdentityId = identityUserId
            };
            
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Created user in custom table: {UserId}", user.user_id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user in custom table");
                return RedirectToAction("Error", "Home");
            }
        }
        
        // Now we have a valid user ID from our custom table
        var goals = _goalRepository.GetGoalsByUserId(user.user_id);
        var viewModel = new GoalListViewModel
        {
            Goals = goals
        };
        return View(viewModel);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        // Default target date is 30 days from now
        return View(new GoalViewModel 
        { 
            TargetDate = DateTime.Now.AddDays(30),
            Category = "General",
            Priority = ViewModels.Priority.Medium // Explicitly reference the namespace
        });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GoalViewModel model)
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        try
        {
            // Get user ID directly from claims
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(identityUserId))
            {
                _logger.LogError("Identity user ID not found in claims for user {Username}", username);
                return RedirectToAction("Error", "Home");
            }
            
            // Find or create user in custom table
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
            
            if (user == null)
            {
                // Create the user in our custom table
                user = new Stride.Data.DatabaseModels.User
                {
                    username = username,
                    email = User.FindFirstValue(ClaimTypes.Email) ?? username,
                    password_hash = "IDENTITY_MANAGED",
                    IdentityId = identityUserId
                };
                
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Created user in custom table during goal creation: {UserId}", user.user_id);
            }
            
            // Ensure the priority exists in the database
            int priorityId = await EnsurePriorityExists(model.Priority);
            
            // Get category ID (create if not exists)
            string categoryName = model.Category ?? "General";
            int categoryId = await EnsureCategoryExists(categoryName);
            
            // Create Goal entity
            var goal = new Goal
            {
                title = model.Title,
                description = model.Description,
                start_date = DateTime.Now,
                end_date = model.TargetDate,
                is_completed = model.IsCompleted,
                category_id = categoryId,
                user_id = user.user_id,
                goal_priority_id = priorityId,
                User = user
            };
            
            // Add the goal
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
    public async Task<IActionResult> Edit(int id)
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        
        // Get user ID from identity
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(identityUserId))
        {
            _logger.LogError("Identity user ID not found in claims for user {Username}", username);
            return RedirectToAction("Error", "Home");
        }
        
        // Find user in custom table
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
        
        if (user == null)
        {
            _logger.LogError("User {Username} not found in custom table", username);
            return RedirectToAction("Index", "Dashboard");
        }
        
        var goal = _goalRepository.GetGoalById(id);
        if (goal == null)
        {
            return NotFound();
        }
        
        // Check if the goal belongs to the current user
        if (goal.user_id != user.user_id)
        {
            return Forbid();
        }
        
        // Convert from Goal entity to GoalViewModel
        var viewModel = new GoalViewModel
        {
            Id = goal.goal_id,
            Title = goal.title ?? string.Empty,
            Description = goal.description ?? string.Empty,
            TargetDate = goal.end_date ?? DateTime.Now.AddDays(30),
            IsCompleted = goal.is_completed,
            // Map GoalPriority to Priority enum
            Priority = goal.Priority != null ? 
                      (ViewModels.Priority)(goal.goal_priority_id - 1) : // Explicitly use the ViewModels namespace
                      ViewModels.Priority.Medium, // Explicitly use the ViewModels namespace
            Category = goal.Category?.name ?? "General"
        };
        
        return View(viewModel);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GoalViewModel model)
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        
        // Get user ID from identity
        var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(identityUserId))
        {
            _logger.LogError("Identity user ID not found in claims for user {Username}", username);
            return RedirectToAction("Error", "Home");
        }
        
        // Find user in custom table
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
        
        if (user == null)
        {
            _logger.LogError("User {Username} not found in custom table", username);
            return RedirectToAction("Index", "Dashboard");
        }
        
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
            
            // Check if the goal belongs to the current user
            if (existingGoal.user_id != user.user_id)
            {
                return Forbid();
            }
            
            // Ensure the priority exists in the database
            int priorityId = await EnsurePriorityExists(model.Priority);
            
            // Get category name from model or keep existing
            string categoryName = model.Category ?? "General";
            int categoryId = await EnsureCategoryExists(categoryName);
            
            // Update the existing goal
            existingGoal.title = model.Title;
            existingGoal.description = model.Description;
            existingGoal.end_date = model.TargetDate;
            existingGoal.is_completed = model.IsCompleted;
            existingGoal.goal_priority_id = priorityId;
            
            // Update the goal
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
    public async Task<IActionResult> Delete(int id)
    {
        var username = User.Identity?.Name;
        
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }
        
        try
        {
            // Get user ID from identity
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(identityUserId))
            {
                _logger.LogError("Identity user ID not found in claims for user {Username}", username);
                return RedirectToAction("Error", "Home");
            }
            
            // Find user in custom table
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
            
            if (user == null)
            {
                _logger.LogError("User {Username} not found in custom table", username);
                return RedirectToAction("Index", "Dashboard");
            }
            
            var goal = _goalRepository.GetGoalById(id);
            if (goal == null)
            {
                return NotFound();
            }
            
            // Check if the goal belongs to the current user
            if (goal.user_id != user.user_id)
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
    
    // Helper method to ensure categories exist
    private async Task<int> EnsureCategoryExists(string categoryName)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.name == categoryName);
        
        if (category == null)
        {
            // Create the category
            _logger.LogInformation("Creating category: {CategoryName}", categoryName);
            var newCategory = new Stride.Data.DatabaseModels.Category
            {
                name = categoryName,
                description = $"{categoryName} category"
            };
            
            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            return newCategory.category_id;
        }
        
        return category.category_id;
    }
    
    // Helper method to ensure priority levels exist
    private async Task<int> EnsurePriorityExists(ViewModels.Priority priority)
    {
        // Convert enum to a readable name
        string priorityName = priority.ToString();
        
        // Try to find the priority in the database
        var priorityEntity = await _dbContext.Set<Stride.Data.DatabaseModels.GoalPriority>()
            .FirstOrDefaultAsync(p => p.name == priorityName);
        
        if (priorityEntity == null)
        {
            // Create the priority
            _logger.LogInformation("Creating priority: {PriorityName}", priorityName);
            var newPriority = new Stride.Data.DatabaseModels.GoalPriority
            {
                name = priorityName,
                description = $"{priorityName} priority"
            };
            
            _dbContext.Set<Stride.Data.DatabaseModels.GoalPriority>().Add(newPriority);
            await _dbContext.SaveChangesAsync();
            return newPriority.goal_priority_id;
        }
        
        return priorityEntity.goal_priority_id;
    }
}