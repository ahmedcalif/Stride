using System.ComponentModel.DataAnnotations;
using Stride.Data.Models;
using Stride.Data.Services;

namespace Stride.ViewModels;

public class SettingsViewModel
{
    [Required]
    public int Id { get; set; }
    
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(255, ErrorMessage = "Username cannot exceed 255 characters")]
    public string Username { get; set; } = string.Empty;
    
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "First Name")]
    [MaxLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
    public string? FirstName { get; set; }
    
    [Display(Name = "Last Name")]
    [MaxLength(255, ErrorMessage = "Last name cannot exceed 255 characters")]
    public string? LastName { get; set; }
    
    [Display(Name = "Gender")]
    public Gender? UserGender { get; set; }
    
    [Display(Name = "City")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }
    
    [Display(Name = "Postal Code")]
    [MaxLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; set; }
    
    [Display(Name = "Full Name")]
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    [Display(Name = "Current Password")]
    public string? CurrentPassword { get; set; }
    
    [Display(Name = "New Password")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
    public string? NewPassword { get; set; }
    
    [Display(Name = "Confirm New Password")]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmNewPassword { get; set; }
    
    public string? Theme { get; set; } = "light";
    public bool EmailNotifications { get; set; } = false;
    public bool PushNotifications { get; set; } = false;
    public bool SmsNotifications { get; set; } = false;
    public bool TwoFactorEnabled { get; set; } = false;
    
    public SettingsViewModel()
    {
    }
    public SettingsViewModel(User user)
    {
        Id = user.Id;
        Username = user.Username ?? string.Empty;
        Email = user.Email ?? string.Empty;
        FirstName = user.FirstName;
        LastName = user.LastName;
        
    }
    
    public SettingsViewModel(ApplicationUser user)
    {
        Id = 0;
        try {
            if (int.TryParse(user.Id, out int userId))
                Id = userId;
        } catch {}
        
        Username = user.UserName ?? string.Empty;
        Email = user.Email ?? string.Empty;
        
        UserGender = user.UserGender;
        City = user.City;
        PostalCode = user.PostalCode;

        try {
            if (user.GetType().GetProperty("FirstName") != null)
                FirstName = (string?)user.GetType().GetProperty("FirstName")?.GetValue(user);
            
            if (user.GetType().GetProperty("LastName") != null)
                LastName = (string?)user.GetType().GetProperty("LastName")?.GetValue(user);
        } catch {}
        
    }
    

    public void UpdateUser(User user)
    {
        user.Username = Username;
        user.Email = Email;
        user.FirstName = FirstName;
        user.LastName = LastName;
        
    
        if (!string.IsNullOrEmpty(NewPassword))
        {
            user.Password = NewPassword;
        }
    }
    public void UpdateApplicationUser(ApplicationUser user)
    {
        user.UserName = Username;
        user.Email = Email;
        

        user.UserGender = UserGender;
        user.City = City;
        user.PostalCode = PostalCode;
        try {
            var userType = user.GetType();
            
            if (userType.GetProperty("FirstName") != null)
                userType.GetProperty("FirstName")?.SetValue(user, FirstName);
            
            if (userType.GetProperty("LastName") != null)
                userType.GetProperty("LastName")?.SetValue(user, LastName);
        } catch {}
        
        user.TwoFactorEnabled = TwoFactorEnabled;
    }
}