using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;

namespace Stride.Data.Models.SQLRepository
{
    public class SQLGoalRepository : IGoalRepository 
    {
        private readonly ApplicationDbContext _dbContext;
        
        public SQLGoalRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        private Goals ConvertToGoals(Goal dbGoal)
        {
            Priority priorityEnum = Priority.Medium; 
            if (dbGoal.Priority != null)
            {
                switch (dbGoal.Priority.name?.ToLower())
                {
                    case "low":
                        priorityEnum = Priority.Low;
                        break;
                    case "high":
                        priorityEnum = Priority.High;
                        break;
                    case "medium":
                    default:
                        priorityEnum = Priority.Medium;
                        break;
                }
            }

            return new Goals
            {
                Id = dbGoal.goal_id, 
                Title = dbGoal.title,
                Description = dbGoal.description, 
                TargetDate = dbGoal.end_date ?? DateTime.Now.AddDays(30),
                IsCompleted = dbGoal.is_completed,
                CategoryId = dbGoal.category_id,
                Category = dbGoal.Category?.name ?? "General",
                Priority = priorityEnum, 
                Username = dbGoal.User?.username
            };
        }
        
        private Goal ConvertToDbGoal(Goals goal)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.username == goal.Username);
            if (user == null)
            {
                throw new Exception($"User '{goal.Username}' not found");
            }
            
            var categoryId = GetOrCreateCategory(goal.Category ?? "General");
            
            return new Goal
            {
                goal_id = goal.Id,
                title = goal.Title, 
                description = goal.Description,
                start_date = DateTime.Now,
                end_date = goal.TargetDate,
                is_completed = goal.IsCompleted,
                category_id = categoryId, 
                user_id = user.user_id,
                User = user,
                goal_priority_id = GetOrCreatePriority(goal.Priority)
            };
        }
        private int GetOrCreateCategory(string categoryName)
        {
            var category = _dbContext.Categories
                .FirstOrDefault(c => c.name.ToLower() == categoryName.ToLower());
                
            if (category != null)
            {
                return category.category_id;
            }
            
            var newCategory = new Category
            {
                name = categoryName,
                description = $"Category for {categoryName} goals"
            };
            
            _dbContext.Categories.Add(newCategory);
            try
            {
                _dbContext.SaveChanges();
                return newCategory.category_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                
                var defaultCategory = _dbContext.Categories
                    .FirstOrDefault(c => c.name.ToLower() == "general");
                    
                if (defaultCategory != null)
                {
                    return defaultCategory.category_id;
                }
                
                var generalCategory = new Category
                {
                    name = "General",
                    description = "Default category for goals"
                };
                
                _dbContext.Categories.Add(generalCategory);
                _dbContext.SaveChanges();
                return generalCategory.category_id;
            }
        }
        
        private int GetOrCreatePriority(Priority priorityEnum)
        {
            string priorityName;
            switch (priorityEnum)
            {
                case Priority.Low:
                    priorityName = "Low";
                    break;
                case Priority.High:
                    priorityName = "High";
                    break;
                case Priority.Medium:
                default:
                    priorityName = "Medium";
                    break;
            }
            
            var priority = _dbContext.GoalPrioritiy
                .FirstOrDefault(p => p.name.ToLower() == priorityName.ToLower());
                
            if (priority != null)
            {
                return priority.goal_priority_id;
            }
            
            // Create default priorities if they don't exist
            try
            {
                var defaultPriority = new GoalPriority { name = priorityName };
                _dbContext.GoalPrioritiy.Add(defaultPriority);
                _dbContext.SaveChanges();
                return defaultPriority.goal_priority_id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating priority: {ex.Message}");
                
                
                var anyPriority = _dbContext.GoalPrioritiy.FirstOrDefault();
                return anyPriority?.goal_priority_id ?? 1;
            }
        }

        public IEnumerable<Goals> GetAllGoals() 
        {
            return _dbContext.Goals
                .Include(g => g.User)
                .Include(g => g.Category)
                .Include(g => g.Priority)
                .Select(g => ConvertToGoals(g))
                .ToList();
        } 

        public Goals? GetGoalById(int id) 
        {
            var dbGoal = _dbContext.Goals
                .Include(g => g.User)
                .Include(g => g.Category)
                .Include(g => g.Priority)
                .FirstOrDefault(g => g.goal_id == id);
            
            return dbGoal != null ? ConvertToGoals(dbGoal) : null;
        } 

        public Goals Add(Goals goal) 
        {
            try
            {
                var dbGoal = ConvertToDbGoal(goal);
                _dbContext.Goals.Add(dbGoal);
                _dbContext.SaveChanges();
                return ConvertToGoals(dbGoal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding goal: {ex.Message}");
                throw;
            }
        }

        public Goals Update(Goals goal)
        {
            try
            {
                var existingGoal = _dbContext.Goals
                    .Include(g => g.User)
                    .Include(g => g.Category)
                    .Include(g => g.Priority)
                    .FirstOrDefault(g => g.goal_id == goal.Id);
                
                if (existingGoal != null)
                {
                    existingGoal.title = goal.Title;
                    existingGoal.description = goal.Description;
                    existingGoal.end_date = goal.TargetDate;
                    existingGoal.is_completed = goal.IsCompleted;
                    
                    existingGoal.goal_priority_id = GetOrCreatePriority(goal.Priority);
                    
                    _dbContext.SaveChanges();
                    return ConvertToGoals(existingGoal);
                }
                return goal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating goal: {ex.Message}");
                throw;
            }
        }
        
        public Goals Delete(int id)
        {
            var dbGoal = _dbContext.Goals
                .Include(g => g.User)
                .FirstOrDefault(g => g.goal_id == id);
            
            if (dbGoal != null)
            {
                var goalToReturn = ConvertToGoals(dbGoal);
                _dbContext.Goals.Remove(dbGoal);
                _dbContext.SaveChanges();
                return goalToReturn;
            }
            
            return new Goals { Id = id };
        }
        
        public IEnumerable<Goals> GetIncompleteGoals()
        {
            return _dbContext.Goals
                .Include(g => g.User)
                .Include(g => g.Category)
                .Include(g => g.Priority)
                .Where(g => !g.is_completed) 
                .Select(g => ConvertToGoals(g))
                .ToList();
        }
      public IEnumerable<Goals> GetGoalsByUsername(string username)
        {
            try
            {
                Console.WriteLine($"Attempting to get goals for username: {username}");
                
                var dbGoals = _dbContext.Goals
                    .Include(g => g.User)
                    .Include(g => g.Category)
                    .Include(g => g.Priority)
                    .Where(g => g.User != null && g.User.username == username)
                    .ToList(); 
                
             
                var goals = dbGoals.Select(g => ConvertToGoals(g)).ToList();
                
                Console.WriteLine($"Found {goals.Count} goals for user {username}");
                return goals;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetGoalsByUsername: {ex.Message}");
                return new List<Goals>();
            }
        }   
    }
}