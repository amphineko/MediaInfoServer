using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using PodcastCore.LibMediaInfo;

namespace PodcastCore.AudioBlobInbox.Services
{
    public class MediaInfoService
    {
        private readonly AmazonS3Client _client;

        public MediaInfoService(AmazonS3Client client)
        {
            _client = client;
        }

        public async Task<AudioBlobInfo> GetRemoteMediaInfo(string bucketName, string key,
            CancellationToken cancellationToken)
        {
            var result = await _client.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            }, cancellationToken);

            var tempFilename = Path.GetTempFileName();
            try
            {
                await result.WriteResponseStreamToFileAsync(tempFilename, false, cancellationToken);

                return await GetMediaInfo(tempFilename, cancellationToken);
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }

        private static async Task<AudioBlobInfo> GetMediaInfo(string filename, CancellationToken cancellationToken)
        {
            await using var stream = File.OpenRead(filename);
            return await GetMediaInfo(stream, cancellationToken);
        }

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