using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{ 
  public class UserAchievements {
    [Required]
    [Key]
    public int user_achievement_id { get; set; }

    [Required]
    public DateTime awarded_at { get; set; }

    public string progress { get; set; }

    public bool is_displayed { get; set; }


  }
}