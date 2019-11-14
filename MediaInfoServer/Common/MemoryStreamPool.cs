using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace PodcastCore.MediaInfoServer.Common
{
    public class MemoryStreamPool
    {
        private readonly IProducerConsumerCollection<WeakReference<MemoryStream>> _pool;

        public MemoryStreamPool()
        {
            _pool = new ConcurrentStack<WeakReference<MemoryStream>>();
        }

        public MemoryStream Get(int? initialCapacity = null)
        {
            while (_pool.TryTake(out var reference))
            {
                if (!reference.TryGetTarget(out var stream))
                    continue;

                if (!stream.CanRead || !stream.CanSeek || !stream.CanWrite)
                    continue;

                if (initialCapacity != null)
                    stream.Capacity = Math.Max(stream.Capacity, (int) initialCapacity);
                return stream;
            }

            return initialCapacity == null ? new MemoryStream() : new MemoryStream((int) initialCapacity);
        }

        public void Return(MemoryStream stream)
        {
            Debug.Assert(stream.CanRead && stream.CanSeek && stream.CanWrite, "Stream has already closed");
            stream.SetLength(0);
            _pool.TryAdd(new WeakReference<MemoryStream>(stream));
        }
    }
}