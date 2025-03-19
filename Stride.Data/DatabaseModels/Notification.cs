 
 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
 
 public class Notification
    {
        [Key]
        public int notification_id { get; set; }
        
        [Required]
        public required string content { get; set; }
        
        [Required]
        public DateTime timestamp { get; set; }
        
        public bool is_read { get; set; }
        
        // Foreign key
        public int user_id { get; set; }
        
        [ForeignKey("user_id")]
        public required User User { get; set; }
        
        public int notification_type_id { get; set; }
        
        [ForeignKey("notification_type_id")]
        public  required NotificationType NotificationType { get; set; }
    }
}
