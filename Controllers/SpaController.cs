using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SpaController : Controller
{
    public IActionResult Login()
    {
         return View("~/Areas/Identity/Pages/Account/_LoginPartial.cshtml");
    }
    
    public IActionResult Register()
    {
         return View("~/Areas/Identity/Pages/Account/_RegisterPartial.cshtml");
    }
}