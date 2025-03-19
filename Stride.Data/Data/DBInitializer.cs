using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;

namespace Stride.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                
                dbContext.Database.EnsureCreated();
                
                SeedHabitFrequencies(dbContext);
                
                EnsureDefaultUser(dbContext);
            }
        }
        
        private static void SeedHabitFrequencies(ApplicationDBContext dbContext)
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
        
        private static void EnsureDefaultUser(ApplicationDBContext dbContext)
        {
            if (!dbContext.Users.Any())
            {
                Console.WriteLine("Creating default user...");
                
                dbContext.Users.Add(new User
                {
                    username = "defaultuser",
                    email = "default@example.com",
                });
                
                dbContext.SaveChanges();
                Console.WriteLine("Default user created successfully.");
            }
        }
    }
}