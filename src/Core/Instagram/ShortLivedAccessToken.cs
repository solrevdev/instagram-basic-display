using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// <para>Short-lived access tokens are valid for 1 hour, but can be exchanged for long-lived tokens. </para>
    /// <para>To get a short-lived access token, implement the Authorization Window into your app. </para>
    /// <para>
    /// After the app user authenticates their identity through the window, Instagram will redirect the
    /// user back to your app and include an Authorization Code, which you can then exchange for a
    /// short-lived access token.
    /// </para>
    /// </summary>
    public class ShortLivedAccessToken
    {
        /// <summary>
        /// The user's app-scoped short-lived Instagram User Access Token.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The user's app-scoped User ID.
        /// </summary>
        [JsonPropertyName("user_id")]
        public long UserId { get; set; }
    }
}
