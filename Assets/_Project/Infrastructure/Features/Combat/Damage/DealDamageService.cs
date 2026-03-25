using _Project.Application.Interfaces;
using _Project.Application.Events.Core;
using _Project.Application.Events.EncounterState;
using _Project.Application.Events.GameState;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;

namespace _Project.Infrastructure.Features.Combat.Damage
{
    public class DealDamageService : IDealDamageUseCase
    {
        private readonly DiceSessionState _diceSessionState;
        private readonly IDiceDamageService _diceDamageService;
        private readonly IEnemyHealthUseCase _enemyHealthUseCase;
        private readonly IDiceRollUseCase _diceRollUseCase;

        public DealDamageService(
            DiceSessionState diceSessionState,
            IDiceDamageService diceDamageService,
            IEnemyHealthUseCase enemyHealthUseCase,
            IDiceRollUseCase diceRollUseCase)
        {
            _diceSessionState = diceSessionState;
            _diceDamageService = diceDamageService;
            _enemyHealthUseCase = enemyHealthUseCase;
            _diceRollUseCase = diceRollUseCase;
        }

        public void DealCurrentDiceDamage()
        {
            if (_diceSessionState.HasDealtThisTurn) return;
            if (_diceSessionState.CurrentTurn > _diceSessionState.MaxTurns) return;

            int totalDamage = 0;

            foreach (DiceState die in _diceSessionState.ActiveDice)
            {
                if (die.CurrentFaceIndex < 0) continue;
                totalDamage += _diceDamageService.CalculateDamage(die);
            }

            if (totalDamage <= 0) return;

            _diceSessionState.HasDealtThisTurn = true;
            _enemyHealthUseCase.ApplyDamage(totalDamage);
            ResetDiceLevels();

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

