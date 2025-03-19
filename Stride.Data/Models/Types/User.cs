using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Stride.Data.Models;


public class User {

  [Required]
  public int Id { get; set; }

  [MaxLength(255)] 
  public string? Name { get; set; }

  [MaxLength(255)]
  public string? Email { get; set;}

  [MaxLength(255)] 
  public string? Password { get; set; }

  [MaxLength(255)]
  public string? Username {get;set;}

  [MaxLength(255)]
  public string? FirstName {get;set;} 


  [MaxLength(255)]
  public string? LastName {get;set;}  
}