using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// Represents a collection of image and video Media on an album Media.
    /// </summary>
    public class Children
    {
        [JsonPropertyName("data")]
        public IList<ChildData> Data { get; set; }
    }
}
