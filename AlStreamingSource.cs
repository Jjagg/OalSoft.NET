using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// A source with a queue of buffers suitable for dynamic sounds or streaming from file.
    /// </summary>
    public class AlStreamingSource : AlSource
    {
        internal AlStreamingSource(uint name, AlContext context) : base(name, context)
        {
        }

        /// <summary>
        /// Queue a buffer.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        public void QueueBuffer(uint name)
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourceQueueBuffers(Name, 1, ref name);
            AlHelper.AlCheckError("alSourceQueueBuffers call failed.");
        }

        /// <summary>
        /// Unqueue one buffer.
        /// </summary>
        /// <returns>Name of the buffer or 0 if no buffer was queued.</returns>
        public uint UnqueueBuffer()
        {
            CheckDisposed();
            Context.MakeCurrent();
            uint name = 0;
            AL10.alSourceUnqueueBuffers(Name, 1, ref name);
            AlHelper.AlCheckError("alSourceUnqueueBuffers call failed.");
            return name;
        }

        /// <summary>
        /// Get the number of queued buffers.
        /// </summary>
        public int QueuedBuffers()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alGetSourcei(Name, AL10.AL_BUFFERS_QUEUED, out var processed);
            AlHelper.AlCheckError("Failed to get number of queued buffers.");
            return processed;
        }

        /// <summary>
        /// Get the number of processed buffers.
        /// </summary>
        public int ProcessedBuffers()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alGetSourcei(Name, AL10.AL_BUFFERS_PROCESSED, out var processed);
            AlHelper.AlCheckError("Failed to get number of processed buffers.");
            return processed;
        }
    }
}
