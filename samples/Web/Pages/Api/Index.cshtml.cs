using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Extensions;

namespace Web
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var user = HttpContext.Session.Get<OAuthResponse>(Strings.SessionKey);
            if (user == null)
            {
                return RedirectToPage("../Auth/Index");
            }

            return Page();
        }
    }
}
