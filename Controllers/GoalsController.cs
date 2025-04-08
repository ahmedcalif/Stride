using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using Stride.Filters;
namespace Stride.Controllers
{
    [VirtualDomFilter]
    [Authorize]
    public class GoalsController : Controller
    {
       private readonly ApplicationDbContext _dbContext;
        private readonly IGoalRepository _goalRepository;
        private readonly ILogger<GoalsController> _logger;
        
        public GoalsController(ApplicationDbContext dbContext, IGoalRepository goalRepository, ILogger<GoalsController> logger)
        {
            _dbContext = dbContext;
            _goalRepository = goalRepository;
            _logger = logger;
        }
        
        // GET: Goals
        public IActionResult Index()
        {
            var userId = GetCurrentUserIdFromClaims();
            var goals = _goalRepository.GetGoalsByUserId(userId);
            
            var viewModel = new GoalListViewModel
            {
                Goals = goals
            };
            
            // Check if request is from AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return partial view for AJAX requests
                return View("_GoalsIndexPartial", viewModel);
            }
            
            // Return full view for normal requests
            return View(viewModel);
        }
        
        // GET: Goals/Create
        public IActionResult Create()
        {
            // Check if request is from AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Return partial view for AJAX requests
                return View("_GoalsCreatePartial", new GoalViewModel());
            }
            
            // Return full view for normal requests
            return View(new GoalViewModel());
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
                // For AJAX requests, return the partial view
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return View("_GoalsCreatePartial", model);
                }
                
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
                
                // For AJAX requests, return JSON response
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Goal created successfully!" });
                }
                
                TempData["Success"] = "Goal created successfully!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal for user {Username}: {Message}", username, ex.Message);
                ModelState.AddModelError("", "Error creating goal. Please try again.");
                
                // For AJAX requests, return the partial view
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return View("_GoalsCreatePartial", model);
                }
                
                return View(model);
            }
        }
     
        public IActionResult Edit(int id)
        {
            var userId = GetCurrentUserIdFromClaims();
            var goal = _goalRepository.GetGoalById(id);
            
            if (goal == null || goal.user_id != userId)
                return NotFound();
            
            var model = new GoalViewModel
            {
                Id = goal.goal_id,
                Title = goal.title,
                Description = goal.description,
                Category = goal.category_id.ToString(),
                TargetDate = goal.end_date ?? DateTime.Now.AddDays(7),
                IsCompleted = goal.is_completed
            };
            
            // Check if request is from AJAX
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return View("_GoalsEditPartial", model);
            }
            
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GoalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return View("_GoalsEditPartial", model);
                }
                
                return View(model);
            }
            
          var userId = GetCurrentUserIdFromClaims();
    var existingGoal = _goalRepository.GetGoalById(model.Id);
    
    
    if (existingGoal == null || existingGoal.user_id != userId)
        return NotFound();


    
    existingGoal.title = model.Title;
    existingGoal.description = model.Description;
    
    string categoryName = model.Category ?? "General";
    int categoryId = await EnsureCategoryExists(categoryName);
    existingGoal.category_id = categoryId;
    
    existingGoal.end_date = model.TargetDate;
    existingGoal.is_completed = model.IsCompleted;
    
    _goalRepository.Update(existingGoal);
    
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Goal updated successfully!" });
            }
            
            TempData["SuccessMessage"] = "Goal updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        // POST: Goals/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var userId = GetCurrentUserIdFromClaims();
            var goal = _goalRepository.GetGoalById(id);
            
            if (goal == null || goal.user_id != userId)
                return NotFound();
            
            _goalRepository.Delete(id);
            
            // For AJAX requests, return JSON response
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Goal deleted successfully!" });
            }
            
            TempData["SuccessMessage"] = "Goal deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        
        private int GetCurrentUserIdFromClaims()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            
            return 1;
        }
        
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
        
        private async Task<int> EnsurePriorityExists(ViewModels.Priority priority)
        {
            string priorityName = priority.ToString();
            
            var priorityEntity = await _dbContext.Set<Stride.Data.DatabaseModels.GoalPriority>()
                .FirstOrDefaultAsync(p => p.name == priorityName);
            
            if (priorityEntity == null)
            {
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
}