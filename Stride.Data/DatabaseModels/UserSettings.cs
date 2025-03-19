using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
    public class UserSetting
    {
        [Key]
        public int user_setting_id { get; set; }
        
        public bool email_notifications { get; set; }
        
        public bool push_notifications { get; set; }
        
        public DateTime created_at { get; set; }
        
        public int theme_id { get; set; }
        
        [ForeignKey("theme_id")]
        public Theme Theme { get; set; }
        
        public int user_id { get; set; }
        
        [ForeignKey("user_id")]
        public User User { get; set; }
    }
}