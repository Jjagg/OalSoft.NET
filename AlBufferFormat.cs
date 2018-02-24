namespace OalSoft.NET
{
    // TODO soft-oal surround

    /// <summary>
    /// Format of data set on a buffer. <seealso cref="AlBuffer.BufferData(uint,OalSoft.NET.AlBufferFormat,byte[],int)" />
    /// </summary>
    public enum AlBufferFormat
    {
        /// <summary>
        /// Single-channel 8-bit PCM.
        /// </summary>
        Mono8=0x1100,
        /// <summary>
        /// Single-channel 16-bit PCM.
        /// </summary>
        Mono16=0x1101,
        /// <summary>
        /// Single-channel IEEE floating-point.
        /// </summary>
        MonoFloat32=0x10010,
        /// <summary>
        /// Dual-channel interleaved 8-bit PCM.
        /// </summary>
        Stereo8=0x1102,
        /// <summary>
        /// Dual-channel interleaved 16-bit PCM.
        /// </summary>
        Stereo16=0x1103,
        /// <summary>
        /// Dual-channel interleaved IEEE floating-point.
        /// </summary>
        StereoFloat32=0x10011,
    }
}
