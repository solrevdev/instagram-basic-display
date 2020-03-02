using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Solrevdev.InstagramBasicDisplay.Core;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Web.Extensions;

namespace Web.Pages.MediaList
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly InstagramApi _api;
        public List<Media> Media { get; } = new List<Media>();

        public IndexModel(ILogger<IndexModel> logger, InstagramApi api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("MediaList/Index called");

            var response = HttpContext.Session.Get<OAuthResponse>(Strings.SessionKey);
            if (response == null)
            {
                return RedirectToLoginPage();
            }

            var media = await _api.GetMediaListAsync(response).ConfigureAwait(false);
            _logger.LogInformation("Initial media response returned with [{count}] records ", media.Data.Count);

            Media.Add(media);

            while (!string.IsNullOrWhiteSpace(media?.Paging?.Next))
            {
                var next = media?.Paging?.Next;
                var count = media?.Data?.Count;
                _logger.LogInformation("Getting next page [{next}]", next);

                media = await _api.GetMediaListAsync(next).ConfigureAwait(false);

                _logger.LogInformation("next media response returned with [{count}] records ", count);

                Media.Add(media);
            }

            return Page();
        }

        private IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("../Auth/Index");
        }
    }
}
