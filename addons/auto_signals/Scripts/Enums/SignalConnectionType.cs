namespace Aspecty.AutoSignals
{
    /// <summary>
    /// Defines the different types of signal connections available
    /// </summary>
    public enum SignalConnectionType
    {
        /// <summary>
        /// Normal signal connection (immediate execution)
        /// </summary>
        Normal,

        /// <summary>
        /// Deferred signal connection (executed at end of frame)
        /// </summary>
        Deferred,

        /// <summary>
        /// One-shot connection (disconnects after first emission)
        /// </summary>
        OneShot,
    }
}
