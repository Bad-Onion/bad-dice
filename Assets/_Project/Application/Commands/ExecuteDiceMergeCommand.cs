using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using Zenject;

namespace _Project.Application.Commands
{
    public class ExecuteDiceMergeCommand : ICommand
    {
        private readonly IDiceMergeUseCase _diceMergeUseCase;
        private readonly string _diceId;

        public ExecuteDiceMergeCommand(IDiceMergeUseCase diceMergeUseCase, string diceId)
        {
            _diceMergeUseCase = diceMergeUseCase;
            _diceId = diceId;
        }

        public bool IsValid()
        {
            return _diceMergeUseCase != null && !string.IsNullOrWhiteSpace(_diceId);
        }

        public void Execute()
        {
            _diceMergeUseCase.ExecuteMerge(_diceId);
        }

        public class Factory : PlaceholderFactory<string, ExecuteDiceMergeCommand>
        {
        }
    }
}

