using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
   public class NotificationType
    {
        [Key]
        public int notification_type_id { get; set; }
        
        [Required]
        public  required string name { get; set; }

        
        public required string description { get; set; }
        
        public ICollection<Notification> Notifications { get; set; }
    }
    
}