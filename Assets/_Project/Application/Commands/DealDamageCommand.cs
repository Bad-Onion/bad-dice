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

        public bool IsValid()
        {
            return _dealDamageUseCase != null;
        }

        public void Execute()
        {
            _dealDamageUseCase.DealCurrentDamage();
        }
    }
}

