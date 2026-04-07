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

        public ValidationResult Validate()
        {
            if (_diceMergeUseCase == null)
            {
                return ValidationResult.Failure("DiceMergeUseCaseMissing", "Dice merge use case is not available.");
            }

            return !string.IsNullOrWhiteSpace(_diceId)
                ? ValidationResult.Success()
                : ValidationResult.Failure("DiceIdMissing", "Dice id is required to execute merge.");
        }

        public CommandResult Execute()
        {
            _diceMergeUseCase.ExecuteMerge(_diceId);
            return CommandResult.Success();
        }

        public class Factory : PlaceholderFactory<string, ExecuteDiceMergeCommand>
        {
        }
    }
}

