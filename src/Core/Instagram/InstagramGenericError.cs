using System.Text.Json.Serialization;

namespace Solrevdev.InstagramBasicDisplay.Core.Instagram
{
    internal class InstagramGenericError
    {
        [JsonPropertyName("error")]
        public InstagramError Error { get; set; }
    }
}
