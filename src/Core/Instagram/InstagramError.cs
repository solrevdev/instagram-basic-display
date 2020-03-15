using System;
using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    public class InstagramError
    {
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type of the error.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the code of the error.
        /// </summary>
        [JsonPropertyName("code")]
        public int? Code { get; set; }

        /// <summary>
        /// Additional information about the error.
        /// </summary>
        [JsonPropertyName("error_subcode")]
        public int? ErrorSubcode { get; set; }

        /// <summary>
        /// Internal support identifier. When reporting a bug related to a Graph API call, include the fbtrace_id to help us find log data for debugging.
        /// </summary>
        [JsonPropertyName("fbtrace_id")]
        public string FbTraceId { get; set; }
    }
}
