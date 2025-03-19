using Stride.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Stride.ViewModels
{
    public class HabitViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "The Title field is required.")]
    [StringLength(254, ErrorMessage = "Title cannot exceed 254 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The Description field is required.")]
    [StringLength(254, ErrorMessage = "Description cannot exceed 254 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "The Frequency field is required.")]
    [Display(Name = "Frequency")]
    public Frequency Frequency { get; set; }
    
   [Required(ErrorMessage = "The Reminder Time field is required.")]
[Display(Name = "Reminder Time")]
[DataType(DataType.Time)]
[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
public DateTime ReminderTime { get; set; }
    
    [Display(Name = "IsActive")]
    public bool IsActive { get; set; } = true;
    
    public List<SelectListItem> FrequencyOptions { get; set; } = new List<SelectListItem>();
    
}
}