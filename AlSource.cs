using System;
using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// Base class for OpenAL sources.
    /// </summary>
    public abstract class AlSource : IDisposable
    {
        internal uint Name;

        /// <summary>
        /// The context that owns this source.
        /// </summary>
        public AlContext Context { get; private set; }

        /// <summary>
        /// Get or set the gain of this source.
        /// </summary>
        public float Gain
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcef(Name, AL10.AL_GAIN, out var value);
                return value;
            }
            set
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alSourcef(Name, AL10.AL_GAIN, value);
            }
        }

        /// <summary>
        /// Get or set the pitch of this source.
        /// </summary>
        public float Pitch
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcef(Name, AL10.AL_PITCH, out var value);
                return value;
            }
            set
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alSourcef(Name, AL10.AL_PITCH, value);
            }
        }

        /// <summary>
        /// Get or set whether this source loops the current buffer.
        /// </summary>
        public bool Looping
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcei(Name, AL10.AL_LOOPING, out var value);
                return value == AL10.AL_TRUE;
            }
            set
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alSourcei(Name, AL10.AL_PITCH, value ? AL10.AL_TRUE : AL10.AL_FALSE);
            }
        }

        /// <summary>
        /// Get the current state of the source.
        /// </summary>
        public SourceState SourceState
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcei(Name, AL10.AL_SOURCE_STATE, out var value);
                return (SourceState) value;
            }
        }

        /// <summary>
        /// Get or set the playback position in seconds. Position is relative to the beginning of the queued buffers.
        /// </summary>
        public float SecOffset
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcef(Name, AL11.AL_SEC_OFFSET, out var value);
                return value;
            }
            set
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alSourcef(Name, AL11.AL_SEC_OFFSET, value);
            }
        }

        /// <summary>
        /// Get or set the playback position in samples. Position is relative to the beginning of the queued buffers.
        /// </summary>
        public int SampleOffset
        {
            get
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alGetSourcei(Name, AL11.AL_SAMPLE_OFFSET, out var value);
                return value;
            }
            set
            {
                CheckDisposed();
                Context.MakeCurrent();
                AL10.alSourcei(Name, AL11.AL_SAMPLE_OFFSET, value);
            }
        }

        /// <summary>
        /// Create an <see cref="AlSource"/>.
        /// </summary>
        /// <param name="name">Name of the source.</param>
        /// <param name="context">Context that owns the source.</param>
        protected AlSource(uint name, AlContext context)
        {
            Name = name;
            Context = context;
        }

        /// <summary>
        /// Play this source.
        /// </summary>
        public void Play()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourcePlay(Name);
        }

        /// <summary>
        /// Pause this source.
        /// </summary>
        public void Pause()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourcePause(Name);
        }

        /// <summary>
        /// Stop this source.
        /// </summary>
        public void Stop()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourceStop(Name);
        }

        /// <summary>
        /// Rewind this source.
        /// </summary>
        public void Rewind()
        {
            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourceRewind(Name);
        }

        internal void OnContextDestroyed()
        {
            ContextDestroyed?.Invoke(this, EventArgs.Empty);
            Context = null;
            Name = 0;
        }

        /// <summary>
        /// Invoked when the context of this source is destroyed.
        /// </summary>
        public event EventHandler<EventArgs> ContextDestroyed;

        protected void CheckDisposed()
        {
            if (Name == 0)
                throw new ObjectDisposedException(GetType().FullName, "Can't access disposed source.");
        }

        private void ReleaseUnmanagedResources()
        {
            if (Name == 0)
                return;
            Context.MakeCurrent();
            Context.DeleteSource(this);
            Context = null;
            Name = 0;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~AlSource()
        {
            ReleaseUnmanagedResources();
        }
    }
}
