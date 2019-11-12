using System;
using System.Runtime.InteropServices;

namespace PodcastCore.LibMediaInfo
{
    internal static class LibMediaInfo
    {
        /// <summary>
        ///     typedef void* MediaInfo_New();
        /// </summary>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_New();

        /// <summary>
        ///     void MediaInfo_Delete(void*);
        /// </summary>
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfo_Delete(IntPtr Handle);

        /// <summary>
        ///     char* MediaInfo_Get(void*, size_t StreamKind, size_t StreamNumber, const char* Parameter,
        ///     size_t KindOfInfo, size_t KindOfSearch);
        /// </summary>
        [DllImport("MediaInfo.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr MediaInfo_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            [MarshalAs(UnmanagedType.LPWStr)] string Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);

        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_Get(IntPtr Handle, IntPtr StreamKind, IntPtr StreamNumber,
            IntPtr Parameter, IntPtr KindOfInfo, IntPtr KindOfSearch);

        /// <summary>
        ///     size_t MediaInfo_Open_Buffer_Init(void*, uint64_t File_Size, uint64_t File_Offset);
        /// </summary>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Open_Buffer_Init(IntPtr Handle, ulong File_Size, ulong File_Offset);

        /// <summary>
        ///     size_t MediaInfo_Open_Buffer_Continue(void*, MediaInfo_int8u* Buffer, size_t Buffer_Size);
        /// </summary>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Open_Buffer_Continue(IntPtr Handle, IntPtr Buffer, IntPtr Buffer_Size);

        /// <summary>
        ///     uint64_t MediaInfo_Open_Buffer_Continue_GoTo_Get(void*);
        /// </summary>
        /// <remarks>
        ///     MediaInfoDLL.cs defines Int64 for its return value.
        /// </remarks>
        [DllImport("MediaInfo.dll")]
        public static extern ulong MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

        /// <summary>
        ///     size_t MediaInfo_Open_Buffer_Finalize(void*);
        /// </summary>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Open_Buffer_Finalize(IntPtr Handle);
    }
}