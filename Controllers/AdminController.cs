using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Stride.Data.Services;

namespace Stride.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if the role already exists
            bool roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            
            if (!roleExists)
            {
                // Create new role
                IdentityRole identityRole = new IdentityRole(model.RoleName);
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                // If there are any errors, add them to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", $"Role '{model.RoleName}' already exists");
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ListRoles()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> EditRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
            return View("NotFound");
        }

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            RoleName = role.Name
        };

        // Retrieve all users in this role
        foreach (var user in _userManager.Users.ToList())
        {
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                model.Users.Add(user.UserName);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditRole(EditRoleViewModel model)
    {
        var role = await _roleManager.FindByIdAsync(model.Id);

        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
            return View("NotFound");
        }
        else
        {
            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
            return View("NotFound");
        }

        var result = await _roleManager.DeleteAsync(role);

        if (result.Succeeded)
        {
            return RedirectToAction("ListRoles");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("ListRoles", _roleManager.Roles.ToList());
    }

    [HttpGet]
    public async Task<IActionResult> ManageUsersInRole(string roleId)
    {
        ViewBag.RoleId = roleId;

        var role = await _roleManager.FindByIdAsync(roleId);
        
        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
            return View("NotFound");
        }

        ViewBag.RoleName = role.Name;

        var model = new List<UserRoleViewModel>();

        foreach (var user in _userManager.Users.ToList())
        {
            var userRoleViewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRoleViewModel.IsSelected = true;
            }
            else
            {
                userRoleViewModel.IsSelected = false;
            }

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageUsersInRole(List<UserRoleViewModel> model, string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);

        if (role == null)
        {
            ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
            return View("NotFound");
        }

        for (int i = 0; i < model.Count; i++)
        {
            var user = await _userManager.FindByIdAsync(model[i].UserId);

            IdentityResult result = null;

            if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
            {
                result = await _userManager.AddToRoleAsync(user, role.Name);
            }
            else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
            {
                result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
            else
            {
                continue;
            }

            if (result.Succeeded)
            {
                if (i < (model.Count - 1))
                    continue;
                else
                    return RedirectToAction("EditRole", new { Id = roleId });
            }
        }

        return RedirectToAction("EditRole", new { Id = roleId });
    }

  [HttpGet]
public async Task<IActionResult> Users()
{
    var users = _userManager.Users.ToList();
    var userViewModels = new List<UserViewModel>();
    
    foreach (var user in users)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var applicationUser = user as ApplicationUser; 
        
        var userViewModel = new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles.ToList(),
            FirstName = applicationUser?.FirstName,
            LastName = applicationUser?.LastName
        };
        
        userViewModels.Add(userViewModel);
    }
    
    return View(userViewModels);
}

[HttpGet]
public async Task<IActionResult> EditUser(string id)
{
    var user = await _userManager.FindByIdAsync(id);
    
    if (user == null)
    {
        ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
        return View("NotFound");
    }
    
    var applicationUser = user as ApplicationUser;
    var userRoles = await _userManager.GetRolesAsync(user);
    var allRoles = _roleManager.Roles.ToList();
    
    var model = new EditUserViewModel
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        FirstName = applicationUser?.FirstName,
        LastName = applicationUser?.LastName,
        Roles = allRoles.Select(r => new RoleSelection 
        {
            RoleName = r.Name,
            IsSelected = userRoles.Contains(r.Name)
        }).ToList()
    };
    
    return View(model);
}

[HttpPost]
public async Task<IActionResult> EditUser(EditUserViewModel model)
{
    var user = await _userManager.FindByIdAsync(model.Id);
    
    if (user == null)
    {
        ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
        return View("NotFound");
    }
    
    // Update basic user properties
    user.UserName = model.UserName;
    user.Email = model.Email;
    
    if (user is ApplicationUser applicationUser)
    {
        applicationUser.FirstName = model.FirstName;
        applicationUser.LastName = model.LastName;
    }
    
    var result = await _userManager.UpdateAsync(user);
    
    if (!result.Succeeded)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }
    
    // Handle role updates
    var userRoles = await _userManager.GetRolesAsync(user);
    
    // Selected roles that are not currently assigned
    var rolesToAdd = model.Roles
        .Where(r => r.IsSelected && !userRoles.Contains(r.RoleName))
        .Select(r => r.RoleName);
    
    // Current roles that are no longer selected
    var rolesToRemove = model.Roles
        .Where(r => !r.IsSelected && userRoles.Contains(r.RoleName))
        .Select(r => r.RoleName);
    
    await _userManager.AddToRolesAsync(user, rolesToAdd);
    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
    
    return RedirectToAction("Users");
}

[HttpPost]
public async Task<IActionResult> DeleteUser(string id)
{
    var user = await _userManager.FindByIdAsync(id);
    
    if (user == null)
    {
        ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
        return View("NotFound");
    }
    
    // Don't allow deleting the last admin user
    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
    
    if (isAdmin && adminUsers.Count <= 1)
    {
        ModelState.AddModelError("", "Cannot delete the only Admin user");
        return RedirectToAction("Users");
    }
    
    var result = await _userManager.DeleteAsync(user);
    
    if (result.Succeeded)
    {
        return RedirectToAction("Users");
    }
    
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError("", error.Description);
    }
    
    return RedirectToAction("Users");
} 
}