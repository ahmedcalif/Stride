using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{ 
  public class AchievementType {
    [Required] 
    [Key]
    public int achievement_type_id { get; set;}

    [Required]
    [StringLength(100)]
    public string name { get;set;}

    [Required]
    [StringLength(100)]
    public string description { get;set;}
  }
}
