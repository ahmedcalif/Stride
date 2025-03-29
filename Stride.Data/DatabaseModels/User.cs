using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stride.Data.DatabaseModels
{
    public class User
    {
        [Key]
        public int user_id { get; set; }
        
        [Required]
        public string?  email { get; set; }
        
        [Required]
        public string? password_hash { get; set; }

        public string? username { get; set;}
      
        public UserSetting? UserSetting { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Achievement> Achievements { get; set; }
    public string IdentityId { get; internal set; }

    public static implicit operator User(string v)
        {
            throw new NotImplementedException();
        }
    }
}
