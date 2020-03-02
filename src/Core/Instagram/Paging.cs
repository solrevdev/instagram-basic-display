using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    /// <summary>
    /// An object containing paging cursors and next/previous data set retrieval URLs.
    /// </summary>
    public class Paging
    {
        [JsonPropertyName("cursors")]
        public Cursors Cursors { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }
}
