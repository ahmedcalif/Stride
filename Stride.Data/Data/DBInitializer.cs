using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stride.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                Console.WriteLine("Beginning database initialization...");
                
                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    Console.WriteLine("Ensuring database is created...");
                    bool wasCreated = dbContext.Database.EnsureCreated();
                    Console.WriteLine($"Database created: {wasCreated}");
                    
                    var connectionString = dbContext.Database.GetConnectionString();
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var maskedConnectionString = connectionString;
                        if (maskedConnectionString.Contains("password=") || maskedConnectionString.Contains("Password="))
                        {
                            maskedConnectionString = "Connection string contains sensitive information";
                        }
                        Console.WriteLine($"Using connection: {maskedConnectionString}");
                    }
                    
                    Console.WriteLine("Checking if tables exist...");
                    bool hasCategories = TableExists<Category>(dbContext);
                    bool hasGoalPriorities = TableExists<GoalPriority>(dbContext);
                    bool hasGoals = TableExists<Goal>(dbContext);
                    bool hasHabits = TableExists<Habit>(dbContext);
                    bool hasUsers = TableExists<User>(dbContext);
                    bool hasHabitFrequencies = TableExists<HabitFrequency>(dbContext);

                    Console.WriteLine($"Tables exist? Categories: {hasCategories}, GoalPriorities: {hasGoalPriorities}, " +
                                      $"Goals: {hasGoals}, Habits: {hasHabits}, Users: {hasUsers}, HabitFrequencies: {hasHabitFrequencies}");
                    
                    SeedHabitFrequencies(dbContext);
                    SeedCategories(dbContext);
                    SeedGoalPriorities(dbContext);
                    
                    var userCount = dbContext.Set<User>().Count();
                    Console.WriteLine($"User count: {userCount}");
                    
                    if (userCount > 0)
                    {
                        SeedGoals(dbContext);
                        SeedHabits(dbContext);
                    }
                    else
                    {
                        Console.WriteLine("No users found. Skipping Goals and Habits seeding.");
                    }
                    
                    Console.WriteLine("Database initialization completed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR during database initialization: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                throw;
            }
        }
        
        private static bool TableExists<T>(ApplicationDbContext dbContext) where T : class
        {
            try
            {
                var count = dbContext.Set<T>().Count();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public static void SeedHabitFrequencies(ApplicationDbContext dbContext)
        {
            try
            {
                if (!dbContext.HabitFrequency.Any())
                {
                    Console.WriteLine("Seeding HabitFrequency data...");
                    
                    dbContext.HabitFrequency.AddRange(
                        new HabitFrequency { habit_frequency_id = (int)Models.Frequency.Daily, name = "Daily" },
                        new HabitFrequency { habit_frequency_id = (int)Models.Frequency.Weekly, name = "Weekly" },
                        new HabitFrequency { habit_frequency_id = (int)Models.Frequency.Monthly, name = "Monthly" },
                        new HabitFrequency { habit_frequency_id = (int)Models.Frequency.Yearly, name = "Yearly" }
                    );
                    
                    int changes = dbContext.SaveChanges();
                    Console.WriteLine($"HabitFrequency data seeded successfully. {changes} records added.");
                }
                else
                {
                    Console.WriteLine("HabitFrequency data already exists. Skipping seeding.");
                    var frequencies = dbContext.HabitFrequency.ToList();
                    Console.WriteLine($"Existing frequencies: {string.Join(", ", frequencies.Select(f => f.name))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding HabitFrequency data: {ex.Message}");
                throw;
            }
        }
        
        public static void SeedCategories(ApplicationDbContext dbContext)
        {
            try
            {
                if (!dbContext.Set<Category>().Any())
                {
                    Console.WriteLine("Seeding Category data...");
                    
                    dbContext.Set<Category>().AddRange(
                        new Category { name = "Health", description = "Health and wellness related goals" },
                        new Category { name = "Career", description = "Career and professional development goals" },
                        new Category { name = "Finance", description = "Financial goals and milestones" },
                        new Category { name = "Education", description = "Learning and educational objectives" },
                        new Category { name = "Personal", description = "Personal growth and lifestyle goals" }
                    );
                    
                    int changes = dbContext.SaveChanges();
                    Console.WriteLine($"Category data seeded successfully. {changes} records added.");
                }
                else
                {
                    Console.WriteLine("Category data already exists. Skipping seeding.");
                    var categories = dbContext.Set<Category>().ToList();
                    Console.WriteLine($"Existing categories: {string.Join(", ", categories.Select(c => c.name))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding Category data: {ex.Message}");
                throw;
            }
        }
        
        public static void SeedGoalPriorities(ApplicationDbContext dbContext)
        {
            try
            {
                if (!dbContext.Set<GoalPriority>().Any())
                {
                    Console.WriteLine("Seeding GoalPriority data...");
                    
                    dbContext.Set<GoalPriority>().AddRange(
                        new GoalPriority { goal_priority_id = 1, name = "High", description = "Urgent and important goals" },
                        new GoalPriority { goal_priority_id = 2, name = "Medium", description = "Important but not urgent goals" },
                        new GoalPriority { goal_priority_id = 3, name = "Low", description = "Goals with flexible timelines" }
                    );
                    
                    int changes = dbContext.SaveChanges();
                    Console.WriteLine($"GoalPriority data seeded successfully. {changes} records added.");
                }
                else
                {
                    Console.WriteLine("GoalPriority data already exists. Skipping seeding.");
                    var priorities = dbContext.Set<GoalPriority>().ToList();
                    Console.WriteLine($"Existing priorities: {string.Join(", ", priorities.Select(p => p.name))}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding GoalPriority data: {ex.Message}");
                throw;
            }
        }
        
        public static void SeedGoals(ApplicationDbContext dbContext)
        {
            try
            {
                // Only seed if no goals exist
                if (!dbContext.Set<Goal>().Any())
                {
                    Console.WriteLine("Seeding Goal data...");
                    
                    // Get a user to associate with the goals
                    var user = dbContext.Set<User>().FirstOrDefault();
                    
                    // If no users exist, we can't seed goals
                    if (user == null)
                    {
                        Console.WriteLine("No users found. Goals seeding skipped.");
                        return;
                    }
                    
                    Console.WriteLine($"Using user: {user.username} (ID: {user.user_id}) for goal seeding");
                    
                    // Get category IDs
                    var categories = dbContext.Set<Category>().ToList();
                    if (categories.Count == 0)
                    {
                        Console.WriteLine("No categories found. Goals seeding skipped.");
                        return;
                    }
                    
                    var healthCategoryId = categories.FirstOrDefault(c => c.name == "Health")?.category_id ?? categories[0].category_id;
                    var careerCategoryId = categories.FirstOrDefault(c => c.name == "Career")?.category_id ?? categories[0].category_id;
                    var financeCategoryId = categories.FirstOrDefault(c => c.name == "Finance")?.category_id ?? categories[0].category_id;
                    var educationCategoryId = categories.FirstOrDefault(c => c.name == "Education")?.category_id ?? categories[0].category_id;
                    var personalCategoryId = categories.FirstOrDefault(c => c.name == "Personal")?.category_id ?? categories[0].category_id;
                    
                    // Get priority IDs
                    var priorities = dbContext.Set<GoalPriority>().ToList();
                    if (priorities.Count == 0)
                    {
                        Console.WriteLine("No priorities found. Goals seeding skipped.");
                        return;
                    }
                    
                    var highPriorityId = priorities.FirstOrDefault(p => p.name == "High")?.goal_priority_id ?? priorities[0].goal_priority_id;
                    var mediumPriorityId = priorities.FirstOrDefault(p => p.name == "Medium")?.goal_priority_id ?? priorities[0].goal_priority_id;
                    var lowPriorityId = priorities.FirstOrDefault(p => p.name == "Low")?.goal_priority_id ?? priorities[0].goal_priority_id;
                    
                    // Create sample goals
                    var goals = new List<Goal>
                    {
                        new Goal 
                        { 
                            title = "Run a marathon", 
                            description = "Train and complete a full marathon",
                            start_date = DateTime.Now,
                            end_date = DateTime.Now.AddMonths(6),
                            is_completed = false,
                            category_id = healthCategoryId,
                            user_id = user.user_id,
                            goal_priority_id = highPriorityId,
                            User = user
                        },
                        new Goal 
                        { 
                            title = "Get certification", 
                            description = "Complete professional certification",
                            start_date = DateTime.Now,
                            end_date = DateTime.Now.AddMonths(3),
                            is_completed = false,
                            category_id = careerCategoryId,
                            user_id = user.user_id,
                            goal_priority_id = highPriorityId,
                            User = user
                        },
                        new Goal 
                        { 
                            title = "Save for vacation", 
                            description = "Save $3000 for summer vacation",
                            start_date = DateTime.Now,
                            end_date = DateTime.Now.AddMonths(5),
                            is_completed = false,
                            category_id = financeCategoryId,
                            user_id = user.user_id,
                            goal_priority_id = mediumPriorityId,
                            User = user
                        },
                        new Goal 
                        { 
                            title = "Learn a new language", 
                            description = "Reach conversational level in Spanish",
                            start_date = DateTime.Now,
                            end_date = DateTime.Now.AddYears(1),
                            is_completed = false,
                            category_id = educationCategoryId,
                            user_id = user.user_id,
                            goal_priority_id = lowPriorityId,
                            User = user
                        },
                        new Goal 
                        { 
                            title = "Read 12 books", 
                            description = "Read one book per month for a year",
                            start_date = DateTime.Now,
                            end_date = DateTime.Now.AddYears(1),
                            is_completed = false,
                            category_id = personalCategoryId,
                            user_id = user.user_id,
                            goal_priority_id = mediumPriorityId,
                            User = user
                        }
                    };
                    
                    dbContext.Set<Goal>().AddRange(goals);
                    int changes = dbContext.SaveChanges();
                    Console.WriteLine($"Goal data seeded successfully. {changes} records added.");
                }
                else
                {
                    Console.WriteLine("Goal data already exists. Skipping seeding.");
                    var goalCount = dbContext.Set<Goal>().Count();
                    Console.WriteLine($"Existing goal count: {goalCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding Goal data: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
        
        public static void SeedHabits(ApplicationDbContext dbContext)
        {
            try
            {
                // Only seed if no habits exist
                if (!dbContext.Set<Habit>().Any())
                {
                    Console.WriteLine("Seeding Habit data...");
                    
                    // Get a user to associate with the habits
                    var user = dbContext.Set<User>().FirstOrDefault();
                    
                    // If no users exist, we can't seed habits
                    if (user == null)
                    {
                        Console.WriteLine("No users found. Habits seeding skipped.");
                        return;
                    }
                    
                    Console.WriteLine($"Using user: {user.username} (ID: {user.user_id}) for habit seeding");
                    
                    // Get frequency IDs
                    var frequencies = dbContext.HabitFrequency.ToList();
                    if (frequencies.Count == 0)
                    {
                        Console.WriteLine("No habit frequencies found. Habits seeding skipped.");
                        return;
                    }
                    
                    var dailyFrequencyId = frequencies.FirstOrDefault(f => f.name == "Daily")?.habit_frequency_id ?? frequencies[0].habit_frequency_id;
                    var weeklyFrequencyId = frequencies.FirstOrDefault(f => f.name == "Weekly")?.habit_frequency_id ?? frequencies[0].habit_frequency_id;
                    var monthlyFrequencyId = frequencies.FirstOrDefault(f => f.name == "Monthly")?.habit_frequency_id ?? frequencies[0].habit_frequency_id;
                    
                    // Create sample habits
                    var now = DateTime.Now;
                    var habits = new List<Habit>
                    {
                        new Habit 
                        { 
                            title = "Morning meditation",
                            description = "15 minutes of mindfulness meditation",
                            reminder_time = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0),
                            created_at = now,
                            updated_at = now,
                            user = user,
                            habit_frequency_id = dailyFrequencyId
                        },
                        new Habit 
                        { 
                            title = "Drink water",
                            description = "Drink 8 glasses of water",
                            reminder_time = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0),
                            created_at = now,
                            updated_at = now,
                            user = user,
                            habit_frequency_id = dailyFrequencyId
                        },
                        new Habit 
                        { 
                            title = "Exercise",
                            description = "30 minutes of cardio or strength training",
                            reminder_time = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0),
                            created_at = now,
                            updated_at = now,
                            user = user,
                            habit_frequency_id = dailyFrequencyId
                        },
                        new Habit 
                        { 
                            title = "Meal planning",
                            description = "Plan meals for the upcoming week",
                            reminder_time = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0),
                            created_at = now,
                            updated_at = now,
                            user = user,
                            habit_frequency_id = weeklyFrequencyId
                        },
                        new Habit 
                        { 
                            title = "Budget review",
                            description = "Review and adjust monthly budget",
                            reminder_time = new DateTime(now.Year, now.Month, 1, 18, 0, 0),
                            created_at = now,
                            updated_at = now,
                            user = user,
                            habit_frequency_id = monthlyFrequencyId
                        }
                    };
                    
                    dbContext.Set<Habit>().AddRange(habits);
                    int changes = dbContext.SaveChanges();
                    Console.WriteLine($"Habit data seeded successfully. {changes} records added.");
                }
                else
                {
                    Console.WriteLine("Habit data already exists. Skipping seeding.");
                    var habitCount = dbContext.Set<Habit>().Count();
                    Console.WriteLine($"Existing habit count: {habitCount}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding Habit data: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }
        
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();
                    
                    string[] roleNames = { "Admin", "User" };
                    
                    foreach (var roleName in roleNames)
                    {
                        try
                        {
                            var roleExists = await roleManager.RoleExistsAsync(roleName);
                            if (!roleExists)
                            {
                                await roleManager.CreateAsync(new IdentityRole(roleName));
                                logger.LogInformation($"Created role: {roleName}");
                                Console.WriteLine($"Created role: {roleName}");
                            }
                            else
                            {
                                Console.WriteLine($"Role {roleName} already exists");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Error creating role {roleName}");
                            Console.WriteLine($"Error creating role {roleName}: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding roles: {ex.Message}");
                throw;
            }
        }
    }
}