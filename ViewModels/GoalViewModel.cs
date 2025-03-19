using System;
using System.ComponentModel.DataAnnotations;
using Stride.Data.Models;

namespace Stride.ViewModels;
// this file is for Create and Editing Goals and connecting them to a view in the most strict typed 
public class GoalViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Target Date")]
    [DataType(DataType.Date)]
    public DateTime TargetDate { get; set; }

    [Required]
    public Priority Priority { get; set; }

    public string? Category { get; set; }

    [Display(Name = "IsCompleted")]
    public bool IsCompleted { get; set; }
}