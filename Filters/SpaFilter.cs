using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stride.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class VirtualDomFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.HttpContext.Items["IsSpaRequest"] = true;

                if (context.Result is ViewResult viewResult)
                {
                    viewResult.ViewData["IsVirtualDomRequest"] = true;
                }
            }
        }
    }
}