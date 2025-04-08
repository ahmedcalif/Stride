using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;
using Stride.Data.Models;

namespace Stride.Data.Models.SQLRepository
{
    public class SQLHabitRepository : IHabitRepository 
    {
        private readonly ApplicationDbContext _dbContext;
        
        public SQLHabitRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        
        private Habits? ConvertToHabits(Habit dbHabit) 
        {
            if (dbHabit == null)
                return null;
                
            try
            {
               var frequency = dbHabit.HabitFrequency != null 
            ? (Frequency)dbHabit.habit_frequency_id 
            : Frequency.Daily;

                return new Habits 
                {
                    Id = dbHabit.habit_id,
                    Title = dbHabit.title ?? string.Empty,
                    Description = dbHabit.description ?? string.Empty,
                    ReminderTime = dbHabit.reminder_time,
                    Frequency = frequency,// Default value if null
                    Username = dbHabit.user?.username ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting to Habits: {ex.Message}");
                return new Habits
                {
                    Id = dbHabit.habit_id,
                    Title = dbHabit.title ?? string.Empty,
                    Description = dbHabit.description ?? string.Empty,
                    ReminderTime = dbHabit.reminder_time,
                    Frequency = Frequency.Daily, // Default fallback
                    Username = dbHabit.user?.username ?? string.Empty
                };
            }
        }
        
 private Habit ConvertToDbHabit(Habits habits) 
{
    if (habits == null)
        return null;

    try
    {
        Console.WriteLine($"Converting Habits with ID {habits.Id} and Frequency {habits.Frequency} ({(int)habits.Frequency})");
        
        var username = !string.IsNullOrEmpty(habits.Username) ? habits.Username : "defaultuser";
        var user = _dbContext.Users.FirstOrDefault(u => u.username == username);

        if (user == null)
        {
            user = _dbContext.Users.FirstOrDefault();
            Console.WriteLine($"Warning: User with username {username} not found. Using default user.");
        }

        if (!_dbContext.HabitFrequency.Any())
        {
            Console.WriteLine("No HabitFrequency records found. Creating default frequencies.");
            CreateDefaultFrequencies();
        }
        
        var allFrequencies = _dbContext.HabitFrequency.ToList();
        Console.WriteLine($"Available frequencies: {string.Join(", ", allFrequencies.Select(f => f.habit_frequency_id))}");
        
        var habitFrequency = _dbContext.HabitFrequency
            .FirstOrDefault(hf => hf.habit_frequency_id == (int)habits.Frequency); 

        if (habitFrequency == null)
        {
            habitFrequency = new HabitFrequency
            {
                habit_frequency_id = (int)habits.Frequency,
                name = habits.Frequency.ToString()
            };
            
            _dbContext.HabitFrequency.Add(habitFrequency);
            _dbContext.SaveChanges();
            
            Console.WriteLine($"Created new HabitFrequency with id {habitFrequency.habit_frequency_id}");
        }
            
        return new Habit 
        {
            habit_id = habits.Id, 
            title = habits.Title ?? string.Empty, 
            description = habits.Description ?? string.Empty,
            reminder_time = habits.ReminderTime,
            created_at = DateTime.Now,
            updated_at = DateTime.Now,
            HabitFrequency = habitFrequency,
            habit_frequency_id = habitFrequency.habit_frequency_id, 
            user = user,
        };
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error converting to DbHabit: {ex.Message}");
        throw;
    }
}

private void CreateDefaultFrequencies()
{
    try
    {
        var frequencies = new List<HabitFrequency>
        {
            new HabitFrequency { habit_frequency_id = (int)Frequency.Daily, name = "Daily" },
            new HabitFrequency { habit_frequency_id = (int)Frequency.Weekly, name = "Weekly" },
            new HabitFrequency { habit_frequency_id = (int)Frequency.Monthly, name = "Monthly" },
            new HabitFrequency { habit_frequency_id = (int)Frequency.Yearly, name = "Yearly" }
        };
        
        _dbContext.HabitFrequency.AddRange(frequencies);
        _dbContext.SaveChanges();
        Console.WriteLine("Default frequencies created successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating default frequencies: {ex.Message}");
        throw;
    }
} 
        public List<Habits> GetHabit() 
{
    try 
    {
        // First fetch all habits
        var dbHabits = _dbContext.Habits
            .Include(h => h.user)
            .Include(h => h.HabitFrequency)
            .ToList(); 
            
    
        return dbHabits
            .Select(h => ConvertToHabits(h))
            .Where(h => h != null)
            .ToList();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GetHabit: {ex.Message}");
        return new List<Habits>();
    }
}
        public Habits GetHabitById(int id) 
        {
            try
            {
                var dbHabit = _dbContext.Habits
                    .Include(h => h.user)
                    .Include(h => h.HabitFrequency)
                    .FirstOrDefault(h => h.habit_id == id);
                    
                return dbHabit != null ? ConvertToHabits(dbHabit) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetHabitById: {ex.Message}");
                return null;
            }
        }
        
        public Habits CreateHabit(Habits habits) 
        {
            try
            {
                var dbHabit = ConvertToDbHabit(habits);
                _dbContext.Habits.Add(dbHabit);
                _dbContext.SaveChanges();
                return ConvertToHabits(dbHabit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateHabit: {ex.Message}");
                throw;
            }
        }
        
        public Habits UpdateHabit(Habits habits) 
        {
            try
            {
                var existingHabit = _dbContext.Habits
                    .Include(h => h.user)
                    .Include(h => h.HabitFrequency)
                    .FirstOrDefault(h => h.habit_id == habits.Id);
                
                if (existingHabit != null)
                {
                    existingHabit.title = habits.Title ?? string.Empty;
                    existingHabit.description = habits.Description ?? string.Empty;
                    existingHabit.reminder_time = habits.ReminderTime;
                    existingHabit.updated_at = DateTime.Now;
                    
                    var habitFrequency = _dbContext.HabitFrequency
                        .FirstOrDefault(hf => hf.habit_frequency_id == (int)habits.Frequency);
                    
                    if (habitFrequency != null)
                    {
                        existingHabit.HabitFrequency = habitFrequency;
                    }
                    
                    // Handle user assignment
                    if (!string.IsNullOrEmpty(habits.Username))
                    {
                        var user = _dbContext.Users.FirstOrDefault(u => u.username == habits.Username);
                        if (user != null)
                        {
                            existingHabit.user = user;
                        }
                    }
                    
                    _dbContext.SaveChanges();
                    return ConvertToHabits(existingHabit);
                }
                
                return habits;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateHabit: {ex.Message}");
                throw;
            }
        }
        
        public Habits DeleteHabit(int id) 
        {
            try
            {
                var dbHabit = _dbContext.Habits
                    .Include(h => h.user)
                    .Include(h => h.HabitFrequency)
                    .FirstOrDefault(h => h.habit_id == id);
                
                if (dbHabit != null)
                {
                    var habitToReturn = ConvertToHabits(dbHabit);
                    _dbContext.Habits.Remove(dbHabit);
                    _dbContext.SaveChanges();
                    return habitToReturn;
                }
                
                return new Habits { Id = id };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteHabit: {ex.Message}");
                throw;
            }
        }
    }
}