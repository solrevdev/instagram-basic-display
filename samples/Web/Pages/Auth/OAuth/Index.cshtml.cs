using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Solrevdev.InstagramBasicDisplay.Core;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Web.Extensions;
using Solrevdev.InstagramBasicDisplay.Core.Exceptions;
using System;

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
        public bool ShowMessage => !string.IsNullOrWhiteSpace(Message);
        [TempData]
        public string Message { get; set; }

        public IndexModel(ILogger<IndexModel> logger, InstagramApi api)
        {
            _logger = logger;
            _api = api;
        }

        public async Task<IActionResult> OnGetAsync(string code, string state)
        {
            _logger.LogInformation("Auth/OAuth for state [{state}] returned the code [{code}]", state, code);

            try
            {
                var response = HttpContext.Session.Get<OAuthResponse>(Strings.SessionKey) ?? await _api.AuthenticateAsync(code, state).ConfigureAwait(false);
                if (response == null)
                {
                    Message = "OAutResponse is null. Redirecting to login page";
                    return RedirectToLoginPage();
                }

                var media = await _api.GetMediaListAsync(response).ConfigureAwait(false);
                _logger.LogInformation("Initial media response returned with [{count}] records ", media.Data.Count);
                Media.Add(media);

                //
                //  TODO: toggle the following boolean for a quick and dirty way of getting all a user's media.
                //
                if (true)
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

                Message = $"{UserInfo.Username} has connected to Instagram successfully.";
            }
            catch (InstagramOAuthException ex)
            {
                Message = $"InstagramOAuthException! {ex.Message} ";
                _logger.LogError(ex, "Instagram OAuth error - instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
            }
            catch (InstagramApiException ex)
            {
                Message = $"InstagramApiException! {ex.Message} ";
                _logger.LogError(ex, "Instagram API error - instagram response message : [{message}] error_type : [{type}] error_code : [{code}] error_sub_code : [{subCode}] fb_trace [{fbTrace}]", ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.ErrorSubcode, ex.FbTraceId);
            }
            catch (InstagramException ex)
            {
                Message = $"InstagramException! {ex.Message} ";
                _logger.LogError(ex, "General Instagram error - instagram response message : [{message}] error_type : [{type}] error_code : [{code}] fb_trace [{fbTrace}]", ex.Message, ex, ex.StackTrace, ex.Message, ex.ErrorType, ex.ErrorCode, ex.FbTraceId);
            }
            catch (Exception ex)
            {
                Message = $"Exception! {ex.Message} ";
                _logger.LogError(ex, "Unknown exception calling with message [{message}] and exception [{ex}] and stack [{stack}]", ex.Message, ex, ex.StackTrace);
            }

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
