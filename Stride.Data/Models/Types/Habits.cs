using System.ComponentModel.DataAnnotations;

namespace Stride.Data.Models;


public enum Frequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}


public class Habits {

  [Required]
  public int Id { get; set; }

  [Required]
  [MaxLength(253)]
  public string? Title { get; set; }

  [MaxLength(253)]
  public string? Description { get; set; } 

  public Frequency Frequency {get; set; }

  public string Username { get; set; }

  public DateTime ReminderTime { get; set; }

  
}