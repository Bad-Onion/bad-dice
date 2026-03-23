using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.DiceSimulation;
using _Project.Domain.Entities.Session;
using _Project.Domain.ScriptableObjects.Configuration;
using _Project.Domain.ScriptableObjects.DiceDefinitions;

namespace _Project.Infrastructure.Services
{
    public class DiceRollService : IDiceRollUseCase
    {
        private readonly DiceSessionState _diceSessionState;
        private readonly PlayerRunState _runState;
        private readonly DiceRollConfiguration _diceRollConfiguration;
        private readonly IDiceSimulationService _simulationService;

        public DiceRollService(
            DiceSessionState diceSessionState,
            PlayerRunState runState,
            DiceRollConfiguration diceRollConfiguration,
            IDiceSimulationService simulationService)
        {
            _diceSessionState = diceSessionState;
            _runState = runState;
            _diceRollConfiguration = diceRollConfiguration;
            _simulationService = simulationService;
        }

        public void RequestRoll()
        {
            var diceToRoll = GetDiceToRoll();
            if (!CanRollDice(diceToRoll)) return;

            _diceSessionState.IsRolling = true;

            if (!IsFirstRoll())
            {
                _diceSessionState.RerollsLeft--;
            }

            PerformRoll(diceToRoll);
        }

        public void EndRoll()
        {
            _diceSessionState.IsRolling = false;
            Bus<DiceRollFinishedEvent>.Raise(new DiceRollFinishedEvent());
        }

        public void ResetDice()
        {
            _diceSessionState.IsRolling = false;
            _diceSessionState.RerollsLeft = _runState.RerollsPerTurn;

            foreach (var die in _diceSessionState.ActiveDice)
            {
                die.CurrentFaceIndex = -1;
                die.IsSelectedForReroll = false;
            }

            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }

        public void ToggleDiceRerollSelection(string diceId)
        {
            if (_diceSessionState.IsRolling) return;

            var die = GetDiceToReroll(diceId);

            if (die == null || !HasBeenRolled(die)) return;

            die.IsSelectedForReroll = !die.IsSelectedForReroll;

            Bus<DiceRerollToggledEvent>.Raise(new DiceRerollToggledEvent
            {
                DiceId = diceId,
                IsSelected = die.IsSelectedForReroll
            });
        }

        private bool CanRollDice(List<DiceState> diceToRoll)
        {
            if (_diceSessionState.IsRolling) return false;
            if (!IsFirstRoll() && _diceSessionState.RerollsLeft <= 0) return false;
            if (diceToRoll.Count == 0) return false;

            return true;
        }

        private bool IsFirstRoll()
        {
            return _diceSessionState.ActiveDice.All(die => !HasBeenRolled(die));
        }

        private void PerformRoll(List<DiceState> diceToRoll)
        {
            int[] targetFaceIndices = new int[diceToRoll.Count];
            DiceDefinition[] definitions = new DiceDefinition[diceToRoll.Count];

            for (int i = 0; i < diceToRoll.Count; i++)
            {
                DiceState die = diceToRoll[i];

                ResetZeroedDice(die);

                int randomFaceIndex = die.Dice.Definition.GetRandomFaceIndex();

                die.CurrentFaceIndex = randomFaceIndex;
                die.IsSelectedForReroll = false;

                targetFaceIndices[i] = randomFaceIndex;
                definitions[i] = die.Dice.Definition;
            }

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { TargetFaceIndices = targetFaceIndices });

            DiceSimulationResult simulationResult = SimulateRoll(definitions, targetFaceIndices);

            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent
            {
                SimulationResult = simulationResult,
                RolledDiceIds = diceToRoll.Select(diceState => diceState.Dice.Id).ToList()
            });
        }

        private static bool HasBeenRolled(DiceState die)
        {
            return die.CurrentFaceIndex != -1;
        }

        private static void ResetZeroedDice(DiceState die)
        {
            if (die.Level == 0) die.Level = 1;
        }

        private DiceState GetDiceToReroll(string diceId)
        {
            return _diceSessionState.ActiveDice.FirstOrDefault(dice => dice.Dice.Id == diceId);
        }

        private List<DiceState> GetDiceToRoll()
        {
            if (IsFirstRoll()) return _diceSessionState.ActiveDice.ToList();

            return _diceSessionState.ActiveDice.Where(diceState => diceState.IsSelectedForReroll).ToList();
        }

        private DiceSimulationResult SimulateRoll(DiceDefinition[] definitions, int[] targetFaceIndices)
        {
            int rollCount = definitions.Length;

            return _simulationService.SimulateTrajectory(
                definitions,
                targetFaceIndices,
                GetStartPositions(rollCount),
                GetRandomRotations(rollCount),
                GetRandomForces(rollCount),
                GetRandomTorques(rollCount)
            );
        }

        private Vector3[] GetStartPositions(int count)
        {
            return BuildArray(count, i =>
            {
                Vector3 offset = new Vector3((i - (count / 2f)) * _diceRollConfiguration.spawnSpacing, 0, 0);

                return _diceRollConfiguration.spawnCenter + offset;
            });
        }

        private Vector3[] GetRandomForces(int count)
        {
            return BuildArray(count, _ =>
            {
                Vector3 randomForce = Random.onUnitSphere *
                                      Random.Range(_diceRollConfiguration.minForce, _diceRollConfiguration.maxForce);
                randomForce.y = Mathf.Abs(randomForce.y);

                return randomForce;
            });
        }

        private Vector3[] GetRandomTorques(int count)
        {
            return BuildArray(count, _ => Random.onUnitSphere * _diceRollConfiguration.torqueMultiplier);
        }

        private static Quaternion[] GetRandomRotations(int count)
        {
            return BuildArray(count, _ => Random.rotation);
        }

        private static T[] BuildArray<T>(int count, System.Func<int, T> elementFactory)
        {
            T[] elements = new T[count];

            for (int i = 0; i < count; i++)
            {
                elements[i] = elementFactory(i);
            }

            return elements;
        }
    }
}