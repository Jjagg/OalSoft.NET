namespace OalSoft.NET
{
    /// <summary>
    /// Playback state of an <see cref="AlSource"/>.
    /// </summary>
    public enum SourceState
    {
        /// <summary>
        /// The source is ready to play.
        /// </summary>
        Initial = 0x1011,
        /// <summary>
        /// The source is being played.
        /// </summary>
        Playing = 0x1012,
        /// <summary>
        /// The source is paused. It can be resumed with <see cref="AlSource.Play"/>.
        /// </summary>
        Paused  = 0x1013,
        /// <summary>
        /// The source is stopped.
        /// </summary>
        Stopped = 0x1014
    }
}