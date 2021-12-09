using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Solrevdev.InstagramBasicDisplay.Core.Instagram;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Solrevdev.InstagramBasicDisplay.Core
{
    /// <summary>
    /// <para>The main entry point for talking to the Instagram Basic Display API which allows users of your app to get basic profile information, photos, and videos in their Instagram accounts.</para>
    /// <para>This class provides means to: Get an Instagram User Access Token and permissions from an Instagram user, Get an Instagram user’s profile, Get an Instagram user’s images, videos, and albums</para>
    /// </summary>
    public class InstagramApi
    {
        private readonly InstagramCredentials _appSettings;
        private readonly ILogger<InstagramApi> _logger;
        private readonly InstagramHttpClient _client;

        /// <summary>
        /// Default constructor that takes in the dependencies that it needs to get configuration, logging and external http client calls
        /// </summary>
        /// <param name="appSettings">An instance of <see="InstagramCredentials" /></param>
        /// <param name="logger">An ILogger for this class</param>
        /// <param name="client">A <see="InstagramHttpClient" /> which wraps http calls </param>
        public InstagramApi(IOptions<InstagramCredentials> appSettings, ILogger<InstagramApi> logger, InstagramHttpClient client)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _client = client;
        }

        /// <summary>
        /// <para>
        /// Returns the URL to send the user of your app to in order to begin the OAuth dance in order to get an access token.
        /// </para>
        /// <para>
        /// This will send the user to Instagram's authorization window where they will be told your app is requesting permissions you set when configuring your Instagram application at https://developers.facebook.com
        /// </para>
        /// </summary>
        /// <returns>The url to send the user to</returns>
        public string Authorize()
        {
            AssertInstagramSettings();

            return Authorize("");
        }

        /// <summary>
        /// <para>
        /// Returns the URL to send the user of your app to in order to begin the OAuth dance in order to get an access token.
        /// </para>
        /// <para>
        /// This will send the user to Instagram's authorization window where they will be told your app is requesting permissions you set when configuring your Instagram application at https://developers.facebook.com
        /// </para>
        /// <para>
        /// Note: you can pass a custom string in a state variable and Instagram will return that variable in the callback. This is useful for passing data between your app and Instagram. For example user-id's and such.
        /// </para>
        /// </summary>
        /// <param name="state">A state parameter that when passed to Instagram will get returned in the callback</param>
        /// <returns>The url to send the user to</returns>
        public string Authorize(string state)
        {
            AssertInstagramSettings();

            var redirectUrl = _appSettings.RedirectUrl;

            return $"https://api.instagram.com/oauth/authorize?client_id={_appSettings.ClientId}&redirect_uri={redirectUrl}&scope=user_profile,user_media&response_type=code&state={state}";
        }

        /// <summary>
        /// The authorization code from Instagram is exchanged for short-lived Instagram User Access Token and an <see="OAuthResponse" /> is returned.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns>An OAuthResponse object</returns>
        public async Task<OAuthResponse> AuthenticateAsync(string code, string state)
        {
            AssertInstagramSettings();

            return await AuthenticateAsync(code, state, false).ConfigureAwait(false);
        }

        /// <summary>
        ///  The authorization code from Instagram is exchanged for short-lived Instagram User Access Token or if the <param name="useLongTermAccessToken"></param> boolean is set to true it is exchanged for a long-lived Instagram User Access Token and an <see="OAuthResponse" /> is returned.
        /// </summary>
        /// <param name="code">The authorization code from Instagram returned in the querystring</param>
        /// <param name="state">The optional state parameter sent with the original request to Instagram</param>
        /// <param name="useLongTermAccessToken">true for a long-lived token and false the default short-lived token.</param>
        /// <returns>An OAuthResponse object</returns>
        public async Task<OAuthResponse> AuthenticateAsync(string code, string state, bool useLongTermAccessToken)
        {
            AssertInstagramSettings();

            try
            {
                // exchange auth code for a short-lived token
                var token = await GetShortLivedAccessTokenAsync(code, state).ConfigureAwait(false);
                var accessToken = token.AccessToken;
                var userId = token.UserId.ToString();
                _logger.LogInformation("Access token swapped for code. The token [{token}] and user [{user}]", accessToken, userId);

                // if you know ahead of time that short-lived token is not going to be enough then we can change it for a long-term token straight away
                if (useLongTermAccessToken)
                {
                    var longTermToken = await GetLongLivedAccessTokenAsync(accessToken).ConfigureAwait(false);
                    accessToken = longTermToken.AccessToken;
                    _logger.LogInformation("Access token swapped for long term token. The token [{token}] and type [{type}] expires in [{expires}]", longTermToken.AccessToken, longTermToken.TokenType, longTermToken.ExpiresIn);
                }

                var userInfo = await GetUserAsync(accessToken, userId).ConfigureAwait(false);
                _logger.LogInformation("User returned [{username}] with [{count}] and account type [{type}]", userInfo.Username, userInfo.MediaCount, userInfo.AccountType);

                return new OAuthResponse { AccessToken = accessToken, User = userInfo };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{me}] cannot exchange an authorization code from Instagram for either a short-lived or long-lived Instagram User Access Token with code [{code}] and state [{state}] because [{message}] and exception [{ex}] and stack [{stack}]", nameof(AuthenticateAsync), code, state, ex.Message, ex, ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Exchanges short-lived access token for a long-lived access token
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <returns>A OAuthResponse or null if an error is caught</returns>
        /// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
        public async Task<OAuthResponse> GetLongLivedAccessTokenAsync(OAuthResponse response)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            var token = await GetLongLivedAccessTokenAsync(response.AccessToken).ConfigureAwait(false);
            if (token != null)
            {
                return new OAuthResponse { User = response.User, AccessToken = token.AccessToken };
            }

            return response;
        }

        /// <summary>
        /// Refreshes an existing long-lived Instagram User Access Token for another 60 day one.
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <returns>A refreshed long-lived 60 day access token or null if an error is caught</returns>
        /// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
        public async Task<OAuthResponse> RefreshLongLivedAccessToken(OAuthResponse response)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            var token = await RefreshLongLivedAccessToken(response.AccessToken).ConfigureAwait(false);
            if (token != null)
            {
                //response.AccessToken = token.AccessToken; // TODO: do we set the original as well?

                return new OAuthResponse { User = response.User, AccessToken = token.AccessToken };
            }

            return response;
        }

        /// <summary>
        /// Gets a <see also="UserInfo" /> for either the current user (me) or the user specified with a userId
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <returns>A UserInfo object or null if an error is caught</returns>
        /// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
        public async Task<UserInfo> GetUserAsync(OAuthResponse response)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            if (response.User == null)
            {
                throw new ArgumentNullException(nameof(response.User));
            }

            if (string.IsNullOrWhiteSpace(response.User.Id))
            {
                throw new ArgumentNullException(nameof(response.User.Id));
            }

            return await GetUserAsync(response.AccessToken, response.User.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see also="UserInfo" /> for either the current user (me) or the user specified with a userId
        /// </summary>
        /// <param name="accessToken">An Instagram User Access Token</param>
        /// <returns>A UserInfo object or null if an error is caught</returns>
        public async Task<UserInfo> GetUserAsync(string accessToken)
        {
            AssertInstagramSettings();

            return await GetUserAsync(accessToken, "me").ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see also="UserInfo" /> for either the current user (me) or the user specified with a userId
        /// </summary>
        /// <param name="accessToken">An Instagram User Access Token</param>
        /// <param name="userId">A valid userId</param>
        /// <returns>A UserInfo object or null if an error is caught</returns>
        public async Task<UserInfo> GetUserAsync(string accessToken, string userId)
        {
            AssertInstagramSettings();

            try
            {
                var url = $"https://graph.instagram.com/{userId}?fields=account_type,id,media_count,username&access_token={accessToken}";
                return await _client.GetJsonAsync<UserInfo>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}] and user [{user}]", nameof(GetUserAsync), accessToken, userId);
                throw;
            }
        }

        /// <summary>
        /// Gets a <see also="Media" /> object for a given user
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <returns>A Media object or null if an error is caught</returns>
        /// <exception cref="ArgumentNullException">An Instagram User Access Token is needed</exception>
        public async Task<Media> GetMediaListAsync(OAuthResponse response)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            if (response.User == null)
            {
                throw new ArgumentNullException(nameof(response.User));
            }

            if (string.IsNullOrWhiteSpace(response.User.Id))
            {
                throw new ArgumentNullException(nameof(response.User.Id));
            }

            return await GetMediaListAsync(response.AccessToken, response.User.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see also="Media" /> object for a given user
        /// </summary>
        /// <param name="accessToken">An Instagram User Access Token</param>
        /// <param name="userId">Default to me or pass a valid userId</param>
        /// <returns>A Media object or null if an error is caught</returns>
        public async Task<Media> GetMediaListAsync(string accessToken, string userId)
        {
            AssertInstagramSettings();

            try
            {
                var url = $"https://graph.instagram.com/{userId}/media?fields=caption,id,media_type,media_url,permalink,thumbnail_url,username,timestamp,children{{id,media_type,media_url,permalink,thumbnail_url,username,timestamp}}&access_token={accessToken}";
                return await _client.GetJsonAsync<Media>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}] and user [{user}]", nameof(GetMediaListAsync), accessToken, userId);
                throw;
            }
        }

        /// <summary>
        /// <para>
        /// Gets a <see also="Media" /> object for a given user, this is used when you have been given the full url including access token</para>
        /// <para>
        /// For example when you have an existing media object you have access to Media.Paging.Next which is a url you can pass here to get the next set of media items in a user's instagram feed
        /// </para>
        /// <para>Used for paging through records/</para>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Media> GetMediaListAsync(string url)
        {
            AssertInstagramSettings();

            try
            {
                return await _client.GetJsonAsync<Media>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the url [{url}]", nameof(GetMediaListAsync), url);
                throw;
            }
        }

        /// <summary>
        /// Gets a <see also="Data" /> object via it's unique identifier
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <param name="mediaId">A valid Media Id</param>
        /// <returns>A Data object or null if an error is caught</returns>
        public async Task<Data> GetMediaAsync(OAuthResponse response, string mediaId)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            if (response.User == null)
            {
                throw new ArgumentNullException(nameof(response.User));
            }

            if (string.IsNullOrWhiteSpace(response.User.Id))
            {
                throw new ArgumentNullException(nameof(response.User.Id));
            }

            if (string.IsNullOrWhiteSpace(mediaId))
            {
                throw new ArgumentNullException(nameof(mediaId));
            }

            return await GetMediaAsync(response.AccessToken, mediaId).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see also="Data" /> object via it's unique identifier
        /// </summary>
        /// <param name="accessToken">An Instagram User Access Token</param>
        /// <param name="mediaId">A valid Media Id</param>
        /// <returns>A Data object or null if an error is caught</returns>
        public async Task<Data> GetMediaAsync(string accessToken, string mediaId)
        {
            AssertInstagramSettings();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(mediaId))
            {
                throw new ArgumentNullException(nameof(mediaId));
            }

            try
            {
                var url = $"https://graph.instagram.com/{mediaId}/?fields=caption,id,media_type,media_url,permalink,thumbnail_url,username,timestamp,children{{id,media_type,media_url,permalink,thumbnail_url,username,timestamp}}&access_token={accessToken}";

                return await _client.GetJsonAsync<Data>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}] and media id [{user}]", nameof(GetMediaAsync), accessToken, mediaId);
                throw;
            }
        }

        /// <summary>
        /// Gets a <see also="ChildData" /> object via it's unique identifier
        /// </summary>
        /// <param name="response">A valid <see cref="OAuthResponse"/></param>
        /// <param name="mediaId">A valid ChildMedia Id</param>
        /// <returns>A ChildData object or null if an error is caught</returns>
        public async Task<ChildData> GetChildMediaAsync(OAuthResponse response, string mediaId)
        {
            AssertInstagramSettings();

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (string.IsNullOrWhiteSpace(response.AccessToken))
            {
                throw new ArgumentNullException(nameof(response.AccessToken));
            }

            if (response.User == null)
            {
                throw new ArgumentNullException(nameof(response.User));
            }

            if (string.IsNullOrWhiteSpace(response.User.Id))
            {
                throw new ArgumentNullException(nameof(response.User.Id));
            }

            if (string.IsNullOrWhiteSpace(mediaId))
            {
                throw new ArgumentNullException(nameof(mediaId));
            }

            return await GetChildMediaAsync(response.AccessToken, mediaId).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a <see also="ChildData" /> object via it's unique identifier
        /// </summary>
        /// <param name="accessToken">An Instagram User Access Token</param>
        /// <param name="mediaId">A valid ChildMedia Id</param>
        /// <returns>A ChildData object or null if an error is caught</returns>
        public async Task<ChildData> GetChildMediaAsync(string accessToken, string mediaId)
        {
            AssertInstagramSettings();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(mediaId))
            {
                throw new ArgumentNullException(nameof(mediaId));
            }

            try
            {
                var url = $"https://graph.instagram.com/{mediaId}/?fields=id,media_type,media_url,permalink,thumbnail_url,username,timestamp&access_token={accessToken}";

                return await _client.GetJsonAsync<ChildData>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}] and media id [{user}]", nameof(GetMediaAsync), accessToken, mediaId);
                throw;
            }
        }

        /// <summary>
        /// Exchanges the authorization code from Instagram for a short-lived Instagram User Access Token
        /// </summary>
        /// <param name="code">The authorization code returned from Instagram</param>
        /// <param name="state">The optional state returned from Instagram</param>
        /// <returns>A ShortLivedAccessToken or null if an error is caught</returns>
        private async Task<ShortLivedAccessToken> GetShortLivedAccessTokenAsync(string code, string state)
        {
            AssertInstagramSettings();

            try
            {
                var redirectUrl = _appSettings.RedirectUrl;
                const string url = "https://api.instagram.com/oauth/access_token";
                var parameters = new Dictionary<string, string> { { "client_id", _appSettings.ClientId },
                        { "client_secret", _appSettings.ClientSecret },
                        { "grant_type", "authorization_code" },
                        { "redirect_uri", redirectUrl },
                        { "code", code },
                        { "state", state }
                    };

                return await _client.PostJsonAsync<ShortLivedAccessToken>(url, parameters).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the code [{code}] and the state [{state}]", nameof(GetShortLivedAccessTokenAsync), code, state);
                throw;
            }
        }

        /// <summary>
        /// Exchanges short-lived access token for a long-lived access token
        /// </summary>
        /// <param name="accessToken">The short-lived access token</param>
        /// <returns>A LongLivedAccessToken or null if an error is caught</returns>
        private async Task<LongLivedAccessToken> GetLongLivedAccessTokenAsync(string accessToken)
        {
            AssertInstagramSettings();

            try
            {
                var url = $"https://graph.instagram.com/access_token?grant_type=ig_exchange_token&client_secret={_appSettings.ClientSecret}&access_token={accessToken}";
                return await _client.GetJsonAsync<LongLivedAccessToken>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}]", nameof(GetLongLivedAccessTokenAsync), accessToken);
                throw;
            }
        }

        /// <summary>
        /// Refreshes an existing long-lived Instagram User Access Token for another 60 day one.
        /// </summary>
        /// <param name="accessToken">The existing long-lived access token</param>
        /// <returns>A refreshed long-lived 60 day access token or null if an error is caught</returns>
        private async Task<LongLivedAccessToken> RefreshLongLivedAccessToken(string accessToken)
        {
            AssertInstagramSettings();

            try
            {
                var url = $"https://graph.instagram.com/refresh_access_token?grant_type=ig_refresh_token&access_token={accessToken}";
                return await _client.GetJsonAsync<LongLivedAccessToken>(url).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot [{me}] with the accessToken [{token}]", nameof(RefreshLongLivedAccessToken), accessToken);
                throw;
            }
        }

        /// <summary>
        /// Checks that <see="InstagramCredentials" /> have been set by appsettings.json or similar.
        /// </summary>
        private void AssertInstagramSettings()
        {
            const string missingSettingTemplate = "The {0} is either null or empty please check the {1} section in your appsettings.json";

            if (_appSettings == null)
            {
                throw new ArgumentNullException(nameof(InstagramCredentials), $"The {nameof(InstagramCredentials)} are null please check your appsettings.json file");
            }

            if (string.IsNullOrWhiteSpace(_appSettings.ClientId))
            {
                throw new ArgumentNullException(nameof(_appSettings.ClientId), string.Format(missingSettingTemplate, nameof(_appSettings.ClientId), nameof(InstagramCredentials)));
            }

            if (string.IsNullOrWhiteSpace(_appSettings.ClientSecret))
            {
                throw new ArgumentNullException(nameof(_appSettings.ClientSecret), string.Format(missingSettingTemplate, nameof(_appSettings.ClientSecret), nameof(InstagramCredentials)));
            }

            if (string.IsNullOrWhiteSpace(_appSettings.RedirectUrl))
            {
                throw new ArgumentNullException(nameof(_appSettings.RedirectUrl), string.Format(missingSettingTemplate, nameof(_appSettings.RedirectUrl), nameof(InstagramCredentials)));
            }

            if (string.IsNullOrWhiteSpace(_appSettings.Name))
            {
                throw new ArgumentNullException(nameof(_appSettings.Name), string.Format(missingSettingTemplate, nameof(_appSettings.Name), nameof(InstagramCredentials)));
            }
        }
    }
}
