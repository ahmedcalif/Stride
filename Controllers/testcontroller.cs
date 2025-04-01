using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stride.Data.Models;
using Stride.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



[Route("diagnostic")]
[AllowAnonymous]
public class DiagnosticController : Controller
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Content("Pong! Application is responding.");
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        throw new Exception("Test exception to verify error handling");
    }
}