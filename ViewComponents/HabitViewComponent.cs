using Microsoft.AspNetCore.Mvc;
using Stride.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Stride.ViewComponents
{
    public class HabitSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _dbContext;

        public HabitSummaryViewComponent(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(int userId)
        {
            try
            {
                // Get habits grouped by frequency name directly from the database
                var habitsByFrequency = await _dbContext.Habits
                    .Where(h => h.user.user_id == userId)
                    .Join(_dbContext.HabitFrequency,
                          h => h.habit_frequency_id,
                          f => f.habit_frequency_id,
                          (h, f) => new { Habit = h, Frequency = f })
                    .GroupBy(hf => hf.Frequency.name)
                    .Select(g => new HabitFrequencySummaryViewModel
                    {
                        FrequencyName = g.Key ?? "Unknown",
                        Count = g.Count()
                    })
                    .ToListAsync();

                return View(habitsByFrequency);
            }
            catch (Exception ex)
            {
                // If there's an error, return a simple error view
                var errorModel = new List<HabitFrequencySummaryViewModel>
                {
                    new HabitFrequencySummaryViewModel
                    {
                        FrequencyName = "Error loading data",
                        Count = 0
                    }
                };
                
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in HabitSummaryViewComponent: {ex.Message}");
                
                return View(errorModel);
            }
        }
    }

    // View model class defined within the same file
    public class HabitFrequencySummaryViewModel
    {
        public string FrequencyName { get; set; }
        public int Count { get; set; }
    }
}