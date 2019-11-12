using System.Text.Json.Serialization;

namespace PodcastCore.AudioBlobInbox.Services
{
    public class AudioBlobInfo
    {
        [JsonPropertyName("containerFormat")] public string ContainerFormat { get; set; }

        [JsonPropertyName("duration")] public int Duration { get; set; }
    }
}