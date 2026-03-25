namespace _Project.Application.UseCases
{
    public interface IDiceRollUseCase
    {
        void RequestRoll();
        void EndRoll();
        void ResetDice();
        void ToggleDiceRerollSelection(string diceId);
    }
}