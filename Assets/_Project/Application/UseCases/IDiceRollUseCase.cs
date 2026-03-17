namespace _Project.Application.UseCases
{
    public interface IDiceRollUseCase
    {
        void RequestRoll();
        void ResetDice();
        void ToggleDiceRerollSelection(string diceId);
    }
}