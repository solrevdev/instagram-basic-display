using System.Net;
using System.Threading.Tasks;
using Solrevdev.InstagramBasicDisplay.Core;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Web.Extensions;

namespace Web.Pages.Api
{
    /// <summary>
    /// <para>An "/api/" endpoint that a VueJS fetch method calls in order to render the result using client side javascript.</para>
    /// <para>This endpoint needs the user to have already authenticated and to have an <see="OAuthResponse" /> already in HttpSession </para>
    /// </summary>
    public class InstagramModel : PageModel
    {
        private readonly InstagramApi _api;
        private readonly ILogger<InstagramModel> _logger;

        public InstagramModel(ILogger<InstagramModel> logger, InstagramApi api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task<JsonResult> OnGetAsync(string url = "")
        {
            var user = HttpContext.Session.Get<OAuthResponse>(Strings.SessionKey);
            if (user != null)
            {
                Media content;
                if (string.IsNullOrWhiteSpace(url))
                {
                    _logger.LogInformation("[{me}] is calling url[{url}]", nameof(OnGetAsync), url);
                    content = await _api.GetMediaListAsync(user.AccessToken, user.User.Id).ConfigureAwait(false);
                }
                else
                {
                    url = WebUtility.UrlDecode(url);
                    _logger.LogInformation("[{me}] is calling url[{url}]", nameof(OnGetAsync), url);
                    content = await _api.GetMediaListAsync(url).ConfigureAwait(false);
                }

                return new JsonResult(content);
            }
            else
            {
                return new JsonResult(new { error = $"{Strings.SessionKey} is NULL from HttpContext.Session" });
            }
        }
    }
}
