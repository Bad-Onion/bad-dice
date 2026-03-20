namespace _Project.Application.UseCases
{
    // TODO: Change name to something more related to the Inventory view like DicePouchUseCase
    public interface IEncounterPreparationUseCase
    {
        void ToggleDiceEquip(string diceId);
        bool CanStartEncounter();
        void StartEncounter();
    }
}