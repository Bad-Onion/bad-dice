using _Project.Application.Interfaces;
using _Project.Application.UseCases;

namespace _Project.Application.Commands
{
    public class StartEncounterCommand : ICommand
    {
        private readonly IEncounterProgressionUseCase _encounterProgressionUseCase;

        public StartEncounterCommand(IEncounterProgressionUseCase encounterProgressionUseCase)
        {
            _encounterProgressionUseCase = encounterProgressionUseCase;
        }

        public ValidationResult Validate()
        {
            return _encounterProgressionUseCase != null
                ? ValidationResult.Success()
                : ValidationResult.Failure("EncounterProgressionUseCaseMissing", "Encounter progression use case is not available.");
        }

        public CommandResult Execute()
        {
            _encounterProgressionUseCase.StartEncounter();
            return CommandResult.Success();
        }
    }
}

