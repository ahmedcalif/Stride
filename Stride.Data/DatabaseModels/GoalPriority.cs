using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Stride.Data.DatabaseModels {
  public class GoalPriority {

    [Required]
    [Key]
    public int goal_priority_id { get; set; }

    [Required]
    [StringLength(100)]
    public string? name { get; set; }
  }
}