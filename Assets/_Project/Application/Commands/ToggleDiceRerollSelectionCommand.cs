using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using Zenject;

namespace _Project.Application.Commands
{
    public class ToggleDiceRerollSelectionCommand : ICommand
    {
        private readonly IDiceRollUseCase _diceRollUseCase;
        private readonly string _diceId;

        public ToggleDiceRerollSelectionCommand(IDiceRollUseCase diceRollUseCase, string diceId)
        {
            _diceRollUseCase = diceRollUseCase;
            _diceId = diceId;
        }

        public bool IsValid()
        {
            return _diceRollUseCase != null && !string.IsNullOrWhiteSpace(_diceId);
        }

        public void Execute()
        {
            _diceRollUseCase.ToggleDiceRerollSelection(_diceId);
        }

        public class Factory : PlaceholderFactory<string, ToggleDiceRerollSelectionCommand>
        {
        }
    }
}

