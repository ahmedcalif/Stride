using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{ 
  public class Theme {
    [Required]
    [Key]
    public int theme_id { get; set; }

    [Required]
    public string name { get; set;}
  }
}
