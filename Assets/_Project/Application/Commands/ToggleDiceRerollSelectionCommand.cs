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

        public ValidationResult Validate()
        {
            if (_diceRollUseCase == null)
            {
                return ValidationResult.Failure("DiceRollUseCaseMissing", "Dice roll use case is not available.");
            }

            return !string.IsNullOrWhiteSpace(_diceId)
                ? ValidationResult.Success()
                : ValidationResult.Failure("DiceIdMissing", "Dice id is required to toggle reroll selection.");
        }

        public CommandResult Execute()
        {
            _diceRollUseCase.ToggleDiceRerollSelection(_diceId);
            return CommandResult.Success();
        }

        public class Factory : PlaceholderFactory<string, ToggleDiceRerollSelectionCommand>
        {
        }
    }
}

