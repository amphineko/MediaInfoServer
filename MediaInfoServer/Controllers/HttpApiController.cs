using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minio;
using PodcastCore.MediaInfoServer.Common;
using PodcastCore.MediaInfoServer.Services;

namespace PodcastCore.MediaInfoServer.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class HttpApiController : ControllerBase
    {
        private static readonly int BlobExpireTime = (int) TimeSpan.FromMinutes(5).TotalSeconds;

        private static readonly int TempFileThreshold = 1024 * 1024 * 2; // 1MB * 2

        private readonly MinioClient _client;
        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;
        private readonly MediaInfoService _mediaInfoService;
        private readonly MemoryStreamPool _streamPool;

        public HttpApiController(MinioClient client, MediaInfoService mediaInfoService, HttpClient httpClient,
            MemoryStreamPool streamPool, Configuration configuration)
        {
            _client = client;
            _mediaInfoService = mediaInfoService;
            _httpClient = httpClient;
            _streamPool = streamPool;
            _configuration = configuration;
        }

        private async Task<Stream> GetResponseStream(HttpResponseMessage response)
        {
            Stream stream;
            if (response.Content.Headers.ContentLength > TempFileThreshold)
            {
                var filename = Path.GetTempFileName();
                stream = System.IO.File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                stream.SetLength(response.Content.Headers.ContentLength ?? 0);
            }
            else
            {
                stream = _streamPool.Get((int?) response.Content.Headers.ContentLength);
            }

            stream.Seek(0, SeekOrigin.Begin);
            await response.Content.CopyToAsync(stream);
            stream.SetLength(stream.Position);
            return stream;
        }

        private void CloseResponseStream(Stream stream)
        {
            switch (stream)
            {
                case FileStream fileStream:
                    fileStream.Close();
                    System.IO.File.Delete(fileStream.Name);
                    break;
                case MemoryStream memoryStream:
                    _streamPool.Return(memoryStream);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [HttpGet("getMediaInfoFromRemoteBlob")]
        public async Task<IActionResult> GetMediaInfoFromObject([FromQuery(Name = "objectKey")] string objectKey)
        {
            var url = await _client.PresignedGetObjectAsync(_configuration.BucketName, objectKey, BlobExpireTime);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stream = await GetResponseStream(response);
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                return new OkObjectResult(await MediaInfoService.GetMediaInfo(stream, CancellationToken.None));
            }
            finally
            {
                CloseResponseStream(stream);
            }
        }
    }
}