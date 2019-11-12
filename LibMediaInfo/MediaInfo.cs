using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PodcastCore.LibMediaInfo
{
    public class MediaInfo : IDisposable
    {
        private const int BufferReadChunkSize = 1024;

        private readonly IntPtr _handle;

        public MediaInfo()
        {
            _handle = LibMediaInfo.MediaInfo_New();
            Debug.Assert(_handle != (IntPtr) 0);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public string Get(StreamKind streamKind, int streamNumber, string parameter, InfoKind infoKind = InfoKind.Text,
            InfoKind searchKind = InfoKind.Name)
        {
            var result = LibMediaInfo.MediaInfo_Get(_handle, (IntPtr) streamKind, (IntPtr) streamNumber,
                parameter, (IntPtr) infoKind, (IntPtr) searchKind);
            return Marshal.PtrToStringUni(result);
        }

        public async Task OpenStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(BufferReadChunkSize);

            LibMediaInfo.MediaInfo_Open_Buffer_Init(_handle, (ulong) stream.Length, (ulong) stream.Position);

            while (true)
            {
                var count = await stream.ReadAsync(buffer, 0, BufferReadChunkSize, cancellationToken);

                var gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    var bufferPtr = gcHandle.AddrOfPinnedObject();
                    var status = LibMediaInfo.MediaInfo_Open_Buffer_Continue(_handle, bufferPtr, (IntPtr) count);

                    if (((Status) status).HasFlag(Status.Finalized))
                        break;
                }
                finally
                {
                    gcHandle.Free();
                }

                // stop on end-of-file
                if (count == 0) break;

                // query next position of stream to read
                var nextPosition = LibMediaInfo.MediaInfo_Open_Buffer_Continue_GoTo_Get(_handle);

                // keep ongoing read while equals to -1
                if ((long) nextPosition == -1) continue;

                // or, seek to requested position
                stream.Seek((long) nextPosition, SeekOrigin.Begin);
                LibMediaInfo.MediaInfo_Open_Buffer_Init(_handle, (ulong) stream.Length, (ulong) stream.Position);
            }

            Debug.Assert((int) LibMediaInfo.MediaInfo_Open_Buffer_Finalize(_handle) == 0x01);
        }

        private void ReleaseUnmanagedResources()
        {
            LibMediaInfo.MediaInfo_Delete(_handle);
        }

        ~MediaInfo()
        {
            ReleaseUnmanagedResources();
        }

        [Flags]
        private enum Status
        {
            None = 0x00,
            Accepted = 0x01,
            Filled = 0x02,
            Updated = 0x04,
            Finalized = 0x08
        }
    }
}