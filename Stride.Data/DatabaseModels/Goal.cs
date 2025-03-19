using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
   public class Goal
    {
        [Key]
        [Required]
        public int goal_id { get; set; }
        
        [Required]
        public string? title { get; set; }
        
        public string? description { get; set; }
        
        public DateTime start_date { get; set; }
        
        public DateTime? end_date { get; set; }
        
        public bool is_completed { get; set; }
        
        public int category_id { get; set; }
        
        [ForeignKey("category_id")]
        public Category? Category { get; set; }
        
        public int user_id { get; set; }
        
        [ForeignKey("user_id")]
        public required User User { get; set; }

        public int goal_priority_id { get; set; }
        [ForeignKey("goal_priority_id")]
        public GoalPriority? Priority { get; set; }
      
    }
}
