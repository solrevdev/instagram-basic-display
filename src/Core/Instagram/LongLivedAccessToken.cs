using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// <para>Long-lived tokens are valid for 60 days and can be refreshed before they expire. </para>
    /// <para>
    /// Short-lived access tokens can be exchanged for long-lived access tokens through
    /// the GET /access_token endpoint.
    /// </para>
    /// <para>
    /// Once you have a long-lived access token you can refresh it before it expires through
    /// the GET /refresh_access_token endpoint.
    /// </para>
    /// </summary>
    public class LongLivedAccessToken
    {
        /// <summary>
        /// A long-lived Instagram User Access Token.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// bearer
        /// </summary>

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Number of seconds until token expires
        /// </summary>
        /// <value></value>
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
    }
}
