using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LibMediaInfo;

namespace LibMediaInfoTest
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var filename = Path.GetFullPath("audio.m4a");
            Console.WriteLine($"Opening file {filename}");
            await using var file = File.OpenRead(filename);

            using var mediaInfo = new MediaInfo();
            Console.WriteLine("Trying to load file via LibMediaFile");
            await mediaInfo.OpenStreamAsync(file, CancellationToken.None);

            Console.WriteLine($"Container format: {mediaInfo.GetFormat()}");

            var audioStreamCount = mediaInfo.GetStreamCount(StreamKind.Audio);
            Console.WriteLine($"Audio stream count: {audioStreamCount}");

            for (var i = 0; i < audioStreamCount; i++)
            {
                Console.WriteLine($"Audio stream #{i}:");

                var duration = TimeSpan.FromMilliseconds(mediaInfo.GetDuration(StreamKind.Audio, i));
                Console.WriteLine($"\tDuration: {duration:c} ({duration.TotalMilliseconds})ms");

                var codec = mediaInfo.GetCodecId(StreamKind.Audio, i);
                var codecLongName = mediaInfo.GetCodecIdInfo(StreamKind.Audio, i);
                Console.WriteLine($"\tCodec: {codec} ({codecLongName})");
            }
        }
    }
}