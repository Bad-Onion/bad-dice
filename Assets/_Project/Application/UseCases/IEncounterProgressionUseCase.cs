namespace _Project.Application.UseCases
{
    public interface IEncounterProgressionUseCase
    {
        bool IsInitialized { get; }
        void InitializeRunProgression();
        void PrepareCurrentEncounter();
        bool TryAdvanceEncounter();
    }
}

