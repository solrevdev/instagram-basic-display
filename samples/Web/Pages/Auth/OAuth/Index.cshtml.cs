using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Solrevdev.InstagramBasicDisplay.Core;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Web.Extensions;

namespace Web.Pages.Auth.OAuth
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly InstagramApi _api;
        public string Code { get; private set; }
        public string State { get; private set; }
        public UserInfo UserInfo { get; set; }
        public List<Media> Media { get; } = new List<Media>();

        public IndexModel(ILogger<IndexModel> logger, InstagramApi api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task<IActionResult> OnGetAsync(string code, string state)
        {
            _logger.LogInformation("Auth/OAuth for state [{state}] returned the code [{code}]", state, code);

            var response = HttpContext.Session.Get<OAuthResponse>(Strings.SessionKey) ?? await _api.AuthenticateAsync(code, state).ConfigureAwait(false);
            if (response == null)
            {
                return RedirectToLoginPage();
            }

            var media = await _api.GetMediaListAsync(response).ConfigureAwait(false);
            _logger.LogInformation("Initial media response returned with [{count}] records ", media.Data.Count);
            Media.Add(media);

            //
            //  TODO: toggle the following boolean for a quick and dirty way of getting all a user's media.
            //
            if (false)
            {
                while (!string.IsNullOrWhiteSpace(media?.Paging?.Next))
                {
                    var next = media?.Paging?.Next;
                    var count = media?.Data?.Count;
                    _logger.LogInformation("Getting next page [{next}]", next);

                    media = await _api.GetMediaListAsync(next).ConfigureAwait(false);

                    _logger.LogInformation("next media response returned with [{count}] records ", count);

                    Media.Add(media);
                }
            }

            Code = code;
            State = state;
            UserInfo = response.User;
            HttpContext.Session.Set(Strings.SessionKey, response);

            return Page();
        }

        private IActionResult RedirectToLoginPage()
        {
            return RedirectToPage("../Index");
        }

        public async Task<IActionResult> ExchangeShortLivedForLongLived(string code, string state)
        {
            var response = await _api.AuthenticateAsync(code, state).ConfigureAwait(false);
            _logger.LogInformation("response access token {token}", response.AccessToken);

            var longLived = await _api.GetLongLivedAccessTokenAsync(response).ConfigureAwait(false);
            _logger.LogInformation("response access token {token}", longLived.AccessToken);

            var another = await _api.RefreshLongLivedAccessToken(response).ConfigureAwait(false);
            _logger.LogInformation("response access token {token}", another.AccessToken);

            return Page();
        }
    }
}
