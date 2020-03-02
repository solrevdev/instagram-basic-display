using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// <para>Represents an image, video, or album.</para>
    /// <para>The root class when deserializing the json response back from Instagram</para>
    /// </summary>
    public class Media
    {
        /// <summary>
        /// The main data node returned from instagram
        /// </summary>
        [JsonPropertyName("data")]
        public IList<Data> Data { get; set; }

        /// <summary>
        /// Paging information returned back from instagram
        /// </summary>
        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }
    }
}
