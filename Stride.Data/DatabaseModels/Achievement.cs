using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Stride.Data.DatabaseModels 
{
  public class Achievement {
    [Required]
    [Key]
    public int achievement_id { get; set;}

    [Required]
    [StringLength(100)]
    public string? name { get; set; }
    public string? description { get; set; }
    [Required]
    public string? icon_url { get; set;}

    [Required]
    public required string requirements { get; set;}

    public DateTime created_at { get; set;}

    [Required]
    public  required bool is_active { get; set;}

    public int achievement_type_id { get; set; }
[ForeignKey("achievement_type_id")]
public AchievementType achievementType { get; set; }


  }
}