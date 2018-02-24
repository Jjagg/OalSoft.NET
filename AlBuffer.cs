using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// Static class with methods to interact with OpenAL buffers.
    /// </summary>
    public static class AlBuffer
    {
        /// <summary>
        /// Fill a buffer with data.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        /// <param name="format">Format of data in the buffer.</param>
        /// <param name="data">Data as a byte array.</param>
        /// <param name="freq">Playback frequency in samples per second.</param>
        public static void BufferData(uint name, AlBufferFormat format, byte[] data, int freq)
        {
            AL10.alBufferData(name, (int) format, data, data.Length, freq);
            AlHelper.AlAlwaysCheckError("alBufferData call failed.");
        }

        /// <summary>
        /// Fill a buffer with audio data in 8-bit PCM format.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        /// <param name="channels">Indicates if the data is mono or stereo.</param>
        /// <param name="data">8-bit PCM data.</param>
        /// <param name="count">Number of samples to buffer.</param>
        /// <param name="freq">Playback frequency in samples per second.</param>
        public static void BufferData(uint name, Channels channels, byte[] data, int count, int freq)
        {
            var format = channels == Channels.Mono ? AlBufferFormat.Mono8 : AlBufferFormat.Stereo8;
            AL10.alBufferData(name, (int) format, data, count, freq);
            AlHelper.AlAlwaysCheckError("alBufferData call failed.");
        }

        /// <summary>
        /// Fill a buffer with audio data in 16-bit PCM format.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        /// <param name="channels">Indicates if the data is mono or stereo.</param>
        /// <param name="data">16-bit PCM data.</param>
        /// <param name="count">Number of samples to buffer.</param>
        /// <param name="freq">Playback frequency in samples per second.</param>
        public static void BufferData(uint name, Channels channels, short[] data, int count, int freq)
        {
            var format = channels == Channels.Mono ? AlBufferFormat.Mono16 : AlBufferFormat.Stereo16;
            AL10.alBufferData(name, (int) format, data, count * 2, freq);
            AlHelper.AlAlwaysCheckError("alBufferData call failed.");
        }

        /// <summary>
        /// Fill a buffer with audio data in IEEE floating-point format.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        /// <param name="channels">Indicates if the data is mono or stereo.</param>
        /// <param name="data">Floating-point data.</param>
        /// <param name="count">Number of samples to buffer.</param>
        /// <param name="freq">Playback frequency in samples per second.</param>
        public static void BufferData(uint name, Channels channels, float[] data, int count, int freq)
        {
            var format = channels == Channels.Mono ? AlBufferFormat.MonoFloat32 : AlBufferFormat.StereoFloat32;
            AL10.alBufferData(name, (int) format, data, count * 4, freq);
            AlHelper.AlAlwaysCheckError("alBufferData call failed.");
        }
    }
}
