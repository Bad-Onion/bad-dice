namespace _Project.Application.UseCases
{
    public interface IEncounterPreparationUseCase
    {
        void ToggleDiceEquip(string diceId);
        bool CanStartEncounter();
        void StartEncounter();
    }
}