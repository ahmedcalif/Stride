using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Stride.Data.DatabaseModels;

namespace Stride.ViewModels
{
    // For Creating and Editing Goals
    public class GoalViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime TargetDate { get; set; }
        
        [Required]
        public Priority Priority { get; set; }
        
        public string Category { get; set; } = "General";
        
        [Display(Name = "Completed")]
        public bool IsCompleted { get; set; }
    }

   
}