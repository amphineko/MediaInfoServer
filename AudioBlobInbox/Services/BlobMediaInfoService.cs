using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using PodcastCore.LibMediaInfo;

namespace PodcastCore.AudioBlobInbox.Services
{
    public class BlobMediaInfoService
    {
        private readonly AmazonS3Client _client;

        public BlobMediaInfoService(AmazonS3Client client)
        {
            _client = client;
        }

        public async Task<AudioBlobInfo> GetBlobMediaInfo(string bucketName, string key,
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

                await using var stream = File.OpenRead(tempFilename);
                using var mediaInfo = new MediaInfo();
                await mediaInfo.OpenStreamAsync(stream, cancellationToken);

                return new AudioBlobInfo
                {
                    ContainerFormat = mediaInfo.GetFormat(),
                    Duration = mediaInfo.GetDuration(StreamKind.Audio, 0)
                };
            }
            finally
            {
                File.Delete(tempFilename);
            }
        }
    }
}