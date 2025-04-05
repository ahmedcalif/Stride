using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stride.Controllers
{
    public class SpaController : Controller
    {
        private readonly IGoalRepository _goalRepository;
        private readonly IHabitRepository _habitRepository;
        private readonly IUserRepository _userRepository;

        public SpaController(
            IGoalRepository goalRepository,
            IHabitRepository habitRepository,
            IUserRepository userRepository)
        {
            _goalRepository = goalRepository;
            _habitRepository = habitRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult LoadContent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // Default to Dashboard
                return View("~/Views/Dashboard/Index.cshtml");
            }

            // Remove leading slash if present
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            // Simple routing - just return the appropriate view
            if (path.Equals("dashboard", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Dashboard/Index.cshtml");
            }
            else if (path.Equals("goals", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Goals/Index.cshtml");
            }
            else if (path.Equals("goals/create", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Goals/Create.cshtml");
            }
            else if (path.StartsWith("goals/edit/", StringComparison.OrdinalIgnoreCase))
            {
                var id = ExtractIdFromPath(path);
                if (id.HasValue)
                {
                    return RedirectToAction("Edit", "Goals", new { id = id.Value });
                }
            }
            else if (path.Equals("habits", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Habits/Index.cshtml");
            }
            else if (path.Equals("habits/create", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Habits/Create.cshtml");
            }
            else if (path.StartsWith("habits/edit/", StringComparison.OrdinalIgnoreCase))
            {
                var id = ExtractIdFromPath(path);
                if (id.HasValue)
                {
                    return RedirectToAction("Edit", "Habits", new { id = id.Value });
                }
            }
            else if (path.Equals("settings", StringComparison.OrdinalIgnoreCase))
            {
                return View("~/Views/Settings/Index.cshtml");
            }
            else if (path.Equals("admin/users", StringComparison.OrdinalIgnoreCase))
            {
                if (User.IsInRole("Admin") || User.HasClaim(c => c.Type == "ActiveRole" && c.Value == "Admin"))
                {
                    return View("~/Views/Admin/Users.cshtml");
                }
                return View("~/Views/Shared/Error.cshtml");
            }

            // Return 404 error
            return View("~/Views/Shared/Error.cshtml");
        }

        private int? ExtractIdFromPath(string path)
        {
            try
            {
                var parts = path.Split('/');
                if (parts.Length < 3)
                    return null;
                
                if (int.TryParse(parts[2], out int id))
                    return id;
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}