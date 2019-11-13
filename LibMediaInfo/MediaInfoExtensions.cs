using System;

namespace PodcastCore.LibMediaInfo
{
    public static class MediaInfoExtensions
    {
        public static string GetFormat(this MediaInfo media)
        {
            return media.Get(StreamKind.General, 0, "Format");
        }

        public static int GetStreamCount(this MediaInfo media, StreamKind streamKind)
        {
            try
            {
                return Convert.ToInt32(media.Get(streamKind, 0, "StreamCount"));
            }
            catch
            {
                return 0;
            }
        }

        public static string GetCodecId(this MediaInfo media, StreamKind streamKind, int streamNumber)
        {
            return media.Get(streamKind, streamNumber, "CodecID");
        }

        public static string GetCodecIdInfo(this MediaInfo media, StreamKind streamKind, int streamNumber)
        {
            return media.Get(streamKind, streamNumber, "CodecID/Info");
        }

        public static int GetDuration(this MediaInfo media, StreamKind streamKind, int streamNumber)
        {
            try
            {
                return Convert.ToInt32(media.Get(streamKind, streamNumber, "Duration"));
            }
            catch
            {
                return 0;
            }
        }
    }
}