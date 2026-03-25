namespace _Project.Application.UseCases
{
    public interface IDiceMergeUseCase
    {
        void ExecuteAutoMerge(string targetDiceId);
        void EvaluateMergePossibilities();
    }
}