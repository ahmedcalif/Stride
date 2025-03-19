using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{ 
  public class Habit {
       
 [Required]
    [Key]
    public int habit_id { get; set; }

    [Required]
    [StringLength(50)]
    public string title { get; set; }
    [Required]
    [StringLength(100)]
    public string description { get; set; }

    [Required]
    public DateTime reminder_time { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("user_id")]
    public User user{ get; set; }

    public int habit_frequency_id { get; set; }

    [ForeignKey("habit_frequency_id")]
    public HabitFrequency HabitFrequency { get; set; }

        
    }
}
