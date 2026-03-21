namespace _Project.Application.UseCases
{
    public interface IDicePouchUseCase
    {
        void ToggleDiceEquip(string diceId);
        void StartEncounter();
    }
}