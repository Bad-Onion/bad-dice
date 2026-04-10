namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for ensuring run initialization.
    /// Handles preparation and initialization of a player run before gameplay begins.
    /// </summary>
    public interface IRunInitializationUseCase
    {
        /// <summary>
        /// Ensures the run is properly initialized, loading an existing run or creating a new one if necessary.
        /// </summary>
        void EnsureRunInitialized();
    }
}