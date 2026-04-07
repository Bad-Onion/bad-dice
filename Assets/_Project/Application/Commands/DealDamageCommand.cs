using _Project.Application.Interfaces;
using _Project.Application.UseCases;

namespace _Project.Application.Commands
{
    public class DealDamageCommand : ICommand
    {
        private readonly IDealDamageUseCase _dealDamageUseCase;

        public DealDamageCommand(IDealDamageUseCase dealDamageUseCase)
        {
            _dealDamageUseCase = dealDamageUseCase;
        }

        public ValidationResult Validate()
        {
            return _dealDamageUseCase != null
                ? ValidationResult.Success()
                : ValidationResult.Failure("DealDamageUseCaseMissing", "Deal damage use case is not available.");
        }

        public CommandResult Execute()
        {
            _dealDamageUseCase.DealCurrentDamage();
            return CommandResult.Success();
        }
    }
}

