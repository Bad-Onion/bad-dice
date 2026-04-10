using _Project.Application.Events.DiceState;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Session;
using System;
using _Project.Application.Events.DiceInput;
using Zenject;

namespace _Project.Infrastructure.Features.DiceSession.UseCases
{
    public class DiceHoverService : IDiceHoverUseCase, IInitializable, IDisposable
    {
        private readonly IDamageCalculationService _damageCalculationService;
        private readonly DiceSessionState _diceSessionState;
        private readonly IDiceHoverInputSource _diceHoverInputSource;

        public event Action<DiceHoverDetailsUpdatedEvent> DiceHoverDetailsUpdated;

        public DiceHoverService(
            IDamageCalculationService damageCalculationService,
            DiceSessionState diceSessionState,
            IDiceHoverInputSource diceHoverInputSource)
        {
            _damageCalculationService = damageCalculationService;
            _diceSessionState = diceSessionState;
            _diceHoverInputSource = diceHoverInputSource;
        }

        public void Initialize()
        {
            _diceHoverInputSource.DiceHoverChanged += HandleDiceHoverChanged;
        }

        public void Dispose()
        {
            _diceHoverInputSource.DiceHoverChanged -= HandleDiceHoverChanged;
        }

        private void HandleDiceHoverChanged(DiceHoverChangedEvent evt)
        {
            if (!evt.IsHovered)
            {
                DiceHoverDetailsUpdated?.Invoke(new DiceHoverDetailsUpdatedEvent
                {
                    HasDetails = false,
                    DiceId = evt.DiceId
                });
                return;
            }

            bool hasDetails = _damageCalculationService.TryCalculateDiceDamage(
                _diceSessionState.ActiveDice,
                evt.DiceId,
                out int currentValue,
                out int level,
                out int damage);

            if (!hasDetails)
            {
                DiceHoverDetailsUpdated?.Invoke(new DiceHoverDetailsUpdatedEvent
                {
                    HasDetails = false,
                    DiceId = evt.DiceId
                });
                return;
            }

            DiceHoverDetailsUpdated?.Invoke(new DiceHoverDetailsUpdatedEvent
            {
                HasDetails = true,
                DiceId = evt.DiceId,
                CurrentValue = currentValue,
                Level = level,
                Damage = damage
            });
        }
    }
}

