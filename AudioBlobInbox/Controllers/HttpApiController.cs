using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PodcastCore.AudioBlobInbox.Services;

namespace PodcastCore.AudioBlobInbox.Controllers
{
    [ApiController]
    [Route("api/v1/getBlobMediaInfo")]
    public class HttpApiController : ControllerBase
    {
        private readonly BlobMediaInfoService _mediaInfoService;

        public HttpApiController(BlobMediaInfoService mediaInfoService)
        {
            _mediaInfoService = mediaInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlobMediaInfo([FromQuery(Name = "bucket")] string bucketName,
            [FromQuery(Name = "key")] string key)
        {
            var result = await _mediaInfoService.GetBlobMediaInfo(bucketName, key, CancellationToken.None);
            return new OkObjectResult(result);
        }
    }
}