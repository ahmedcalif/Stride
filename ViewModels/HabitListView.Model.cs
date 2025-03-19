using System.Collections.Generic;
using Stride.Data.Models;

namespace Stride.ViewModels
{
    public class HabitListViewModel
    {
        public List<Habits> Habits { get; set; } = new List<Habits>();
        
        public HabitViewModel NewHabit { get; set; } = new HabitViewModel();
    }
}