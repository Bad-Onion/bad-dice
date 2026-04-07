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

        public ValidationResult Validate()
        {
            return _diceRollUseCase != null
                ? ValidationResult.Success()
                : ValidationResult.Failure("DiceRollUseCaseMissing", "Dice roll use case is not available.");
        }

        public CommandResult Execute()
        {
            _diceRollUseCase.ResetDice();
            return CommandResult.Success();
        }
    }
}

