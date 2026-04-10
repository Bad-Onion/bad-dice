using _Project.Application.Interfaces;
using _Project.Application.Events.Core;
using _Project.Application.Events.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;

namespace _Project.Infrastructure.Features.Combat.Damage
{
    public class DealDamageService : IDealDamageUseCase
    {
        private readonly DiceSessionState _diceSessionState;
        private readonly IDamageCalculationService _damageCalculationService;
        private readonly IEnemyHealthUseCase _enemyHealthUseCase;
        private readonly IDiceRollUseCase _diceRollUseCase;

        public DealDamageService(
            DiceSessionState diceSessionState,
            IDamageCalculationService damageCalculationService,
            IEnemyHealthUseCase enemyHealthUseCase,
            IDiceRollUseCase diceRollUseCase)
        {
            _diceSessionState = diceSessionState;
            _damageCalculationService = damageCalculationService;
            _enemyHealthUseCase = enemyHealthUseCase;
            _diceRollUseCase = diceRollUseCase;
        }

        public void DealCurrentDamage()
        {
            if (_diceSessionState.HasDealtThisTurn) return;
            if (_diceSessionState.CurrentTurn > _diceSessionState.MaxTurns) return;

            int totalDamage = _damageCalculationService.CalculateTotalDamage(_diceSessionState.ActiveDice);

            if (totalDamage <= 0) return;

            _diceSessionState.HasDealtThisTurn = true;
            _enemyHealthUseCase.ApplyDamage(totalDamage);

            ResetDiceLevels();
            AdvanceTurnAndNotify();
        }

        private void AdvanceTurnAndNotify()
        {
            bool hasMoreTurns = _diceSessionState.CurrentTurn < _diceSessionState.MaxTurns;
            if (!hasMoreTurns) return;

            _diceSessionState.CurrentTurn++;
            _diceSessionState.HasDealtThisTurn = false;
            _diceRollUseCase.ResetDice();

            Bus<TurnChangedEvent>.Raise(new TurnChangedEvent
            {
                CurrentTurn = _diceSessionState.CurrentTurn,
                MaxTurns = _diceSessionState.MaxTurns
            });
        }

        private void ResetDiceLevels()
        {
            foreach (DiceState die in _diceSessionState.ActiveDice)
            {
                die.Level = 1;
            }
        }
    }
}

