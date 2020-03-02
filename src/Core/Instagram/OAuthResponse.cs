using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// <para>Wraps either a short-lived or long-lived instagram access token and instagram user profile</para>
    /// <para>
    /// It is named and shaped like this as this library was born to replace the Instaharp libray which
    /// talks to the original now deprecated v1 instagram api.
    /// </para>
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        /// A <see cref="UserInfo"/> object
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// Either a short-lived or long-loved instagram user access token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
