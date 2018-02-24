using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// A source with one active buffer that can be set with the <see cref="SetBuffer"/> method.
    /// </summary>
    public class AlStaticSource : AlSource
    {
        internal AlStaticSource(uint name, AlContext context) : base(name, context)
        {
        }

        /// <summary>
        /// Unqueues all buffers and set the given buffer to be the current buffer.
        /// Does nothing in <see cref="AL10.AL_PLAYING"/> and <see cref="AL10.AL_PAUSED"/> states.
        /// </summary>
        /// <param name="name">Name of the buffer to set.</param>
        public void SetBuffer(uint name)
        {
            var ss = SourceState;
            if (ss == SourceState.Playing || ss == SourceState.Paused)
                return;

            CheckDisposed();
            Context.MakeCurrent();
            AL10.alSourcei(Name, AL10.AL_BUFFER, (int) name);
            AlHelper.AlCheckError("Setting buffer failed.");
        }
    }
}