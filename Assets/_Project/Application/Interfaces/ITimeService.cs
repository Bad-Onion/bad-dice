namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing game timescale and time-related operations.
    /// Allows controlling game speed through timescale manipulation (pause, slow-motion, etc.).
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// Sets the game timescale to the specified value.
        /// </summary>
        /// <param name="scale">The timescale multiplier. Use 0 to pause, 1 for normal speed, and values between 0 and 1 for slow-motion.</param>
        void SetTimeScale(float scale);
    }
}