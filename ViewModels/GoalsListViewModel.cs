using System;
using System.ComponentModel.DataAnnotations;
using Stride.Data.Models;
using Stride.Data.DatabaseModels;

namespace Stride.ViewModels;
// for listing goals (strictly typed)
 public class GoalListViewModel
    {
        public IEnumerable<Goal> Goals { get; set; } = new List<Goal>();
    }

    // Priority enum that matches your database GoalPriority values
    // Adjust the values if needed based on your actual database values
    public enum Priority
    {
        Low = 0,     // Assuming this maps to goal_priority_id = 1
        Medium = 1,  // Assuming this maps to goal_priority_id = 2
        High = 2,    // Assuming this maps to goal_priority_id = 3
        Critical = 3 // Assuming this maps to goal_priority_id = 4
    }