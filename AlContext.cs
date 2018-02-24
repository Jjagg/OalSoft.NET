using System;
using System.Collections.Generic;
using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// Managed wrapper for an OpenAL Context. A context is attached to a single device. It owns a collection of
    /// sources that can be played on the device.
    /// </summary>
    public sealed class AlContext : IDisposable
    {
        internal IntPtr Handle { get; private set; }
        private readonly List<AlSource> _sources;

        /// <summary>
        /// Get this context's <see cref="AlDevice"/>.
        /// </summary>
        public AlDevice Device { get; }

        internal AlContext(AlDevice device, IntPtr handle)
        {
            Device = device;
            Handle = handle;
            _sources = new List<AlSource>();
        }

        /// <summary>
        /// Create an <see cref="AlStreamingSource"/> for this context.
        /// </summary>
        public AlStreamingSource CreateStreamingSource()
        {
            ALC10.alcMakeContextCurrent(Handle);
            AL10.alGenSources(1, out var name);
            AlHelper.AlAlwaysCheckError("Call to alGenSources failed.");
            var source = new AlStreamingSource(name, this);
            _sources.Add(source);
            return source;
        }

        /// <summary>
        /// Create an <see cref="AlStaticSource"/> for this context.
        /// </summary>
        public AlStaticSource CreateStaticSource()
        {
            ALC10.alcMakeContextCurrent(Handle);
            AL10.alGenSources(1, out var name);
            AlHelper.AlAlwaysCheckError("Call to alGenSources failed.");
            var source = new AlStaticSource(name, this);
            _sources.Add(source);
            return source;
        }

        internal void DeleteSource(AlSource source)
        {
            if (!_sources.Remove(source))
                throw new InvalidOperationException("Context does not own the given source.");
            AL10.alDeleteSources(1, ref source.Name);
            AlHelper.AlCheckError("Call to alDeleteSources failed.");
            _sources.Remove(source);
        }

        internal void MakeCurrent()
        {
            ALC10.alcMakeContextCurrent(Handle);
        }

        internal void Destroy()
        {
            // notify all sources first
            foreach (var source in _sources)
                source.OnContextDestroyed();
            ALC10.alcDestroyContext(Handle);
        }

        private void ReleaseUnmanagedResources()
        {
            Device.DestroyContext(this);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~AlContext()
        {
            ReleaseUnmanagedResources();
        }
    }
}
