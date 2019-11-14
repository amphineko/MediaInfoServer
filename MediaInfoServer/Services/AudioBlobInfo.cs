using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PodcastCore.MediaInfoServer.Services
{
    public class AudioBlobInfo
    {
        public static readonly IDictionary<string, string> ContainerTypes = new Dictionary<string, string>
        {
            {"MPEG Audio", "audio/mpeg"},
            {"MPEG-4", "audio/mpeg4-generic"}
        };

        [JsonPropertyName("_audioCount")] public int AudioStreamCount { get; set; }

        [JsonPropertyName("container")] public string ContainerType { get; set; }

        [JsonPropertyName("duration")] public int Duration { get; set; }
    }
}