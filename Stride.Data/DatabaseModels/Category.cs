using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{ 
  public class Category
    {
        [Key]
        [Required]
        public int category_id { get; set; }
        
        [Required]
        public string name { get; set; }
        
        public string description { get; set; }
        
        // Navigation property
        public ICollection<Goal> Goals { get; set; }
    }
}