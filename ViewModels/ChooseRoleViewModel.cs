using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stride.ViewModels
{
    public class ChooseRoleViewModel
    {
        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Choose your role")]
        public string SelectedRole { get; set; }
        
        public List<RoleOption> AvailableRoles { get; set; } = new List<RoleOption>();
    }
    
    public class RoleOption
    {
        public string RoleName { get; set; }
        public string DisplayName { get; set; }
    }
}