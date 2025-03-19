using Microsoft.AspNetCore.Mvc;
using Stride.Data.Models;
using Stride.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using Stride.Data.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Stride.Controllers;

public class HabitsController : Controller
{
    private readonly IHabitRepository _habitRepository;
    private readonly ILogger<HabitsController> _logger;

    private readonly ApplicationDBContext _dbContext;

    public HabitsController(IHabitRepository habitRepository, ILogger<HabitsController> logger, ApplicationDBContext dBContext)
    {
        _habitRepository = habitRepository;
        _logger = logger;
        _dbContext = dBContext;
    }

    public IActionResult Index()
    {
        try
        {
            var habits = _habitRepository.GetHabit();
            var viewModel = new HabitListViewModel
            {
                Habits = habits ?? new List<Habits>()
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching habits");
            TempData["Error"] = "Error loading habits: " + ex.Message;
            return View(new HabitListViewModel { Habits = new List<Habits>() });
        }
    }

    [HttpGet]
public IActionResult Create()
{
    var viewModel = new HabitViewModel
    {
        ReminderTime = DateTime.Now 
    };

    try
    {
      
        var frequencies = _dbContext.HabitFrequency.ToList();
        
    
        viewModel.FrequencyOptions = frequencies
            .Select(f => new SelectListItem
            {
                Value = f.habit_frequency_id.ToString(),
                Text = f.name ?? f.habit_frequency_id.ToString() 
            })
            .ToList();
            
        if (!viewModel.FrequencyOptions.Any())
        {
            viewModel.FrequencyOptions = Enum.GetValues(typeof(Frequency))
                .Cast<Frequency>()
                .Select(f => new SelectListItem
                {
                    Value = ((int)f).ToString(),
                    Text = f.ToString()
                })
                .ToList();
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading frequency options");
        
        viewModel.FrequencyOptions = Enum.GetValues(typeof(Frequency))
            .Cast<Frequency>()
            .Select(f => new SelectListItem
            {
                Value = ((int)f).ToString(),
                Text = f.ToString()
            })
            .ToList();
    }
    
    return View(viewModel);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(HabitViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var habit = new Habits
            {
                Title = model.Title,
                Description = model.Description,
                Frequency = model.Frequency,
                ReminderTime = model.ReminderTime,
                Username = User.Identity?.Name ?? "defaultuser" 
            };

            _habitRepository.CreateHabit(habit);
            TempData["Success"] = "Habit created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating habit: {Message}", ex.Message);
            ModelState.AddModelError("", "Error creating habit. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        try
        {
            var habit = _habitRepository.GetHabitById(id);
            if (habit == null)
            {
                return NotFound();
            }

            var viewModel = new HabitViewModel
            {
                Id = habit.Id,
                Title = habit.Title ?? string.Empty,
                Description = habit.Description ?? string.Empty,
                Frequency = habit.Frequency,
                ReminderTime = habit.ReminderTime
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting habit for edit");
            TempData["Error"] = "Error loading habit. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, HabitViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var habit = new Habits
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Frequency = model.Frequency,
                ReminderTime = model.ReminderTime,
                Username = User.Identity?.Name ?? "defaultuser" 
            };

            _habitRepository.UpdateHabit(habit);
            TempData["Success"] = "Habit updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating habit: {Message}", ex.Message);
            ModelState.AddModelError("", "Error updating habit. Please try again.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            _habitRepository.DeleteHabit(id);
            TempData["Success"] = "Habit deleted successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting habit: {Message}", ex.Message);
            TempData["Error"] = "Error deleting habit. Please try again.";
        }

        return RedirectToAction(nameof(Index));
    }
}