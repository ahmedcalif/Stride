using System.ComponentModel.DataAnnotations;
using Stride.Data.DatabaseModels;

namespace Stride.Data.Models;

public enum Priority {
  Low,
  Medium,
  High
}

public class Goals {

[Required]
public int Id { get; set;}


[Required]
[MaxLength(255)]
public string? Title { get; set;}


[MaxLength(255)]
public string? Description { get; set;}

[Required]
public DateTime TargetDate  { get; set;}

public string? Category { get; set; }

public int CategoryId { get; set; }

public Priority Priority { get; set;}

public bool IsCompleted { get; set;}

public string Username { get; set; }

}