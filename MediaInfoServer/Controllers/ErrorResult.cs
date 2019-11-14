namespace PodcastCore.MediaInfoServer.Controllers
{
    public class ErrorResult
    {
        public ErrorResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }
    }
}