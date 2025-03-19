using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stride.Data.Models {

public class AuthModel : PageModel
{
    public IActionResult OnPost()
    {
        return RedirectToPage("/Dashboard");
    }
}

}
