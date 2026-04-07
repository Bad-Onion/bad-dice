using _Project.Application.Interfaces;
using _Project.Application.UseCases;

namespace _Project.Application.Commands
{
    public class ResetDiceCommand : ICommand
    {
        private readonly IDiceRollUseCase _diceRollUseCase;

        public ResetDiceCommand(IDiceRollUseCase diceRollUseCase)
        {
            _diceRollUseCase = diceRollUseCase;
        }

        public bool IsValid()
        {
            return _diceRollUseCase != null;
        }

        public void Execute()
        {
            _diceRollUseCase.ResetDice();
        }
    }
}

