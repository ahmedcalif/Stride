  using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    public string Id { get; set; }
    
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    public List<RoleSelection> Roles { get; set; } = new List<RoleSelection>();
}

public class RoleSelection
{
    public string RoleName { get; set; }
    public bool IsSelected { get; set; }
}