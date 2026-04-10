using System;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.GameState;
using _Project.Application.States.DiceSession;
using _Project.Application.States.Encounter;
using _Project.Application.UseCases;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Run.Session;

namespace _Project.Infrastructure.Features.Combat.Progression
{
    /// <summary>
    /// Initializes combat turn state for the current encounter.
    /// </summary>
    public class EncounterStartService : IEncounterStartUseCase
    {
        private readonly CombatSessionState _combatSessionState;
        private readonly EnemyEncounterState _enemyEncounterState;
        private readonly IRunRepository _runRepository;
        private readonly PlayerRunState _runState;
        private readonly DiceSessionState _diceSessionState;
        private readonly DiceRollState _diceRollState;
        private readonly DiceMergeState _diceMergeState;

        public EncounterStartService(
            CombatSessionState combatSessionState,
            EnemyEncounterState enemyEncounterState,
            IRunRepository runRepository,
            PlayerRunState runState,
            DiceSessionState diceSessionState,
            DiceRollState diceRollState,
            DiceMergeState diceMergeState)
        {
            _combatSessionState = combatSessionState;
            _enemyEncounterState = enemyEncounterState;
            _runRepository = runRepository;
            _runState = runState;
            _diceSessionState = diceSessionState;
            _diceRollState = diceRollState;
            _diceMergeState = diceMergeState;
        }

        public EncounterSnapshot StartEncounter()
        {
            if (!HasEquippedDice()) return null;

            InitializeDiceSessionState();
            InitializeDiceRollState();
            InitializeDiceMergeState();
            PopulateActiveDice();

            _runRepository.SaveRun(_runState, _combatSessionState);

            Bus<TurnChangedEvent>.Raise(new TurnChangedEvent
            {
                CurrentTurn = _diceSessionState.CurrentTurn,
                MaxTurns = _diceSessionState.MaxTurns
            });

            UpdateEncounterState();

            return _enemyEncounterState.Snapshot;
        }

        private void InitializeDiceSessionState()
        {
            _diceSessionState.ActiveDice.Clear();
            _diceSessionState.RerollsLeft = _runState.RerollsPerTurn;
            _diceSessionState.CurrentTurn = 1;
            _diceSessionState.MaxTurns = _runState.TurnsPerFight;
            _diceSessionState.HasDealtThisTurn = false;
        }

        private void InitializeDiceRollState()
        {
            _diceRollState.IsRolling = false;
            _diceRollState.RollPhase = DiceRollPhase.Idle;
            _diceRollState.CurrentTargetFaceIndices = Array.Empty<int>();
            _diceRollState.CurrentSimulationResult = default;
            _diceRollState.CurrentRolledDiceIds.Clear();
        }

        private void InitializeDiceMergeState()
        {
            _diceMergeState.MergeableDiceIds.Clear();
            _diceMergeState.MergeState = MergeState.None;
        }

        private void PopulateActiveDice()
        {
            var equippedDice = _runState.DiceInventory.Where(diceData => diceData.IsEquipped);
            foreach (var ownedDice in equippedDice)
            {
                _diceSessionState.ActiveDice.Add(new DiceState
                {
                    Dice = ownedDice.Dice,
                    Level = 1,
                    CurrentFaceIndex = -1,
                    IsSelectedForReroll = false
                });
            }
        }

        private bool HasEquippedDice()
        {
            return _runState.DiceInventory.Any(diceData => diceData.IsEquipped);
        }

        private void UpdateEncounterState()
        {
            _enemyEncounterState.Phase = EncounterPhase.Active;
            _enemyEncounterState.Snapshot ??= new EncounterSnapshot();
            _enemyEncounterState.Snapshot.Phase = EncounterPhase.Active;
        }
    }
}




