using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace Stride.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        public ErrorController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        [Route("Error/ShowDeveloperPage")]
        public IActionResult ShowDeveloperPage()
        {
            throw new Exception("This is a test exception to demonstrate the developer exception page");
        }
        
        [Route("Error/Staging")]
        public IActionResult Staging()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
            ViewData["StackTrace"] = exception?.StackTrace;
            return View(exception);
        }
        
        [Route("Error/Production")]
        public IActionResult Production()
        {
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
            return View();
        }
        
     [Route("Error/HandleStatusCode/{code:int}")]
public IActionResult HandleStatusCode(int code)
{
    // Set the status code in the response
    Response.StatusCode = code;
    
    ViewData["StatusCode"] = code;
    
    if (_hostingEnvironment.IsDevelopment() || _hostingEnvironment.EnvironmentName == "Testing")
    {
        if (code == 404)
        {
            return View("NotFound_Development");
        }
        return View("Error_Development", code);
    }
    else if (_hostingEnvironment.IsStaging())
    {
        if (code == 404)
        {
            return View("NotFound_Staging");
        }
        return View("Error_Staging", code);
    }
    else
    {
        if (code == 404)
        {
            return View("NotFound_Production");
        }
        return View("Production");
    }
} 
        
        [Route("Error/StatusCode")]
      public new IActionResult StatusCode(int code) 
        {
            // Set the actual HTTP status code in the response
            Response.StatusCode = code;
            
            if (_hostingEnvironment.IsDevelopment() || _hostingEnvironment.EnvironmentName == "Testing")
            {
                throw new Exception($"Status code {code} - Forcing developer exception page");
            }
            
            ViewData["StatusCode"] = code;
            
            if (_hostingEnvironment.IsStaging())
            {
                if (code == 404)
                {
                    return View("NotFound_Staging");
                }
                return View("Error_Staging", code);
            }
            else
            {
                if (code == 404)
                {
                    return View("NotFound_Production");
                }
                return View("Production");
            }
        }
    }
}