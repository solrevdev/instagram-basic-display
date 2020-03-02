using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// <para>Represents an image, video, or album.</para>
    /// </summary>
    public class Data
    {
        /// <summary>
        /// The Media's caption text. Not returnable for Media in albums.
        /// </summary>
        [JsonPropertyName("caption")]
        public string Caption { get; set; }

        /// <summary>
        /// The Media's ID.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The Media's type. Can be IMAGE, VIDEO, or CAROUSEL_ALBUM.
        /// </summary>

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; }

        /// <summary>
        /// The Media's URL.
        /// </summary>
        [JsonPropertyName("media_url")]
        public string MediaUrl { get; set; }

        /// <summary>
        /// The Media's permanent URL.
        /// </summary>
        [JsonPropertyName("permalink")]
        public string Permalink { get; set; }

        /// <summary>
        /// The Media's thumbnail image URL. Only available on VIDEO Media.
        /// </summary>
        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// The Media owner's username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; }

        /// <summary>
        /// The Media's publish date in ISO 8601 format.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Get a list of Media on the Media album. Only available on CAROUSEL_ALBUM Media.
        /// </summary>
        [JsonPropertyName("children")]
        public Children Children { get; set; }
    }
}
