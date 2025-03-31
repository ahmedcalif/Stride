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

namespace Stride.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
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
    public IActionResult ChooseRole() {
      return View();
    }
}