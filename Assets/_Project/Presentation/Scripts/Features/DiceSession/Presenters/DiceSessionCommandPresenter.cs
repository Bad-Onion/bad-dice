using _Project.Application.Commands;

namespace _Project.Presentation.Scripts.Features.DiceSession.Presenters
{
    public class DiceSessionCommandPresenter
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly RequestDiceRollCommand _requestDiceRollCommand;
        private readonly ResetDiceCommand _resetDiceCommand;
        private readonly DealDamageCommand _dealDamageCommand;

        public DiceSessionCommandPresenter(
            CommandProcessor commandProcessor,
            RequestDiceRollCommand requestDiceRollCommand,
            ResetDiceCommand resetDiceCommand,
            DealDamageCommand dealDamageCommand)
        {
            _commandProcessor = commandProcessor;
            _requestDiceRollCommand = requestDiceRollCommand;
            _resetDiceCommand = resetDiceCommand;
            _dealDamageCommand = dealDamageCommand;
        }

        public void RequestRoll()
        {
            _commandProcessor.ExecuteCommand(_requestDiceRollCommand);
        }

        public void RequestReset()
        {
            _commandProcessor.ExecuteCommand(_resetDiceCommand);
        }

        public void RequestDealDamage()
        {
            _commandProcessor.ExecuteCommand(_dealDamageCommand);
        }
    }
}
