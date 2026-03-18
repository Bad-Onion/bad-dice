namespace _Project.Application.UseCases
{
    public interface IDiceMergeUseCase
    {
        void ToggleMergeMode();
        void ToggleDiceMergeSelection(string diceId);
        void SubmitMerge();
        void EvaluateMergePossibilities();
    }
}