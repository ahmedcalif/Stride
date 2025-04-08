using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
    public class HabitFrequency
    {
        [Key]
        public int habit_frequency_id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string name { get; set; }
        
        public ICollection<Habit> Habits { get; set; }
    }
}