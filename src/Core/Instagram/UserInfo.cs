using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// A User�s Profile
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// The User's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The User's account type. Can be BUSINESS, MEDIA_CREATOR, or PERSONAL.
        /// </summary>
        [JsonPropertyName("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        ///The number of Media on the User.This field requires the instagram_graph_user_media permission.
        /// </summary>
        [JsonPropertyName("media_count")]
        public int MediaCount { get; set; }

        /// <summary>
        /// The User's username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
