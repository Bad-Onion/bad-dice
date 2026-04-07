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

        public bool IsValid()
        {
            return _encounterProgressionUseCase != null;
        }

        public void Execute()
        {
            _encounterProgressionUseCase.StartEncounter();
        }
    }
}

