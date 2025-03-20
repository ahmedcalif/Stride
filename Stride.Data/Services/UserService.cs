// Custom class that derives from Identity User

using Microsoft.AspNetCore.Identity;

namespace Stride.Data.Services;

public enum Gender {
  Male,
  Female,
  Other 
}

public class ApplicationUser : IdentityUser {

public Gender? UserGender { get; set; }

 public string? City { get; set; }
    public string? PostalCode { get; set; }

}