using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minio;
using PodcastCore.AudioBlobInbox.Services;

namespace PodcastCore.AudioBlobInbox.Controllers
{
    [ApiController]
    [Route("api/v1/audioBlobs")]
    public class HttpApiController : ControllerBase
    {
        private readonly MinioClient _client;
        private readonly Configuration _configuration;
        private readonly MediaInfoService _mediaInfoService;

        public HttpApiController(MinioClient client, MediaInfoService mediaInfoService, Configuration configuration)
        {
            _client = client;
            _mediaInfoService = mediaInfoService;
            _configuration = configuration;
        }

        private static Task<string> ComputeObjectKey(Stream stream)
        {
            return Task.Run(() =>
            {
                using var hash = SHA1.Create();

                stream.Seek(0, SeekOrigin.Begin);
                return string.Concat(hash.ComputeHash(stream).Select(b => b.ToString("x2")));
            });
        }

        public async Task<IActionResult> CreateBlobFromStream(Stream stream)
        {
            var mediaInfo = await MediaInfoService.GetMediaInfo(stream, CancellationToken.None);
            if (mediaInfo.ContainerType == null)
                return BadRequest(new ErrorResult(400, "Invalid or unsupported audio container"));
            if (mediaInfo.AudioStreamCount != 1)
                return BadRequest(new ErrorResult(400, $"Invalid audio stream count: {mediaInfo.AudioStreamCount}"));
            mediaInfo.ObjectKey = await ComputeObjectKey(stream);

            stream.Seek(0, SeekOrigin.Begin);
            await _client.PutObjectAsync(
                _configuration.BucketName, mediaInfo.ObjectKey,
                stream, stream.Length,
                mediaInfo.ContainerType, new Dictionary<string, string> {{"Duration", mediaInfo.Duration.ToString()}});

            var uriBuilder = new UriBuilder(_configuration.ServiceUrl);
            Path.Combine(uriBuilder.Path, _configuration.BucketName, mediaInfo.ObjectKey);

            return new CreatedResult(uriBuilder.ToString(), mediaInfo);
        }

        [HttpPost]
        [HttpPut]
        [Route("")]
        public async Task<IActionResult> CreateBlob()
        {
            if (Request.Body == null)
                return BadRequest();

            await using var stream = new MemoryStream();
            await Request.Body.CopyToAsync(stream);
            return await CreateBlobFromStream(stream);
        }
    }
}