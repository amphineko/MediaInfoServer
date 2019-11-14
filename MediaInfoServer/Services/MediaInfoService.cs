using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PodcastCore.LibMediaInfo;

namespace PodcastCore.MediaInfoServer.Services
{
    public class MediaInfoService
    {
        private static string GetMediaTypeString(string formatString)
        {
            return AudioBlobInfo.ContainerTypes.TryGetValue(formatString, out var type) ? type : null;
        }

        public static async Task<AudioBlobInfo> GetMediaInfo(Stream stream, CancellationToken cancellationToken)
        {
            using var mediaInfo = new MediaInfo();
            stream.Seek(0, SeekOrigin.Begin);
            await mediaInfo.OpenStreamAsync(stream, cancellationToken);

            return new AudioBlobInfo
            {
                AudioStreamCount = mediaInfo.GetStreamCount(StreamKind.Audio),
                ContainerType = GetMediaTypeString(mediaInfo.GetFormat()),
                Duration = mediaInfo.GetDuration(StreamKind.Audio, 0)
            };
        }
    }
}