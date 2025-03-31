using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                dbContext.Database.EnsureCreated();
                
                SeedHabitFrequencies(dbContext);
            }
        }
        
        private static void SeedHabitFrequencies(ApplicationDbContext dbContext)
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
                
                dbContext.SaveChanges();
                Console.WriteLine("HabitFrequency data seeded successfully.");
            }
        }
        
      
        public static async Task SeedRoles(IServiceProvider serviceProvider)
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
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Error creating role {roleName}");
                    }
                }
            }
        }
    }
}