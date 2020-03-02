using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Solrevdev.InstagramBasicDisplay.Core;
using Microsoft.Extensions.Logging;

namespace Web.Pages.Auth
{
    public class IndexModel : PageModel
    {
        private readonly InstagramApi _api;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(InstagramApi api, ILogger<IndexModel> logger)
        {
            _api = api;
            _logger = logger;
        }

        public ActionResult OnGet()
        {
            var url = _api.Authorize(Strings.StateKey);
            _logger.LogInformation("Auth is redirecting to [{page}]", url);
            return Redirect(url);
        }
    }
}
