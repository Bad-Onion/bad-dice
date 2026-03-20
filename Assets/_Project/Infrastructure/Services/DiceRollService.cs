using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;

namespace _Project.Infrastructure.Services
{
    public class DiceRollService : IDiceRollUseCase
    {
        private readonly DiceSession _diceSession;
        private readonly DiceConfiguration _diceConfiguration;
        private readonly IDiceSimulationService _simulationService;

        public DiceRollService(DiceSession diceSession, DiceConfiguration diceConfiguration,
            IDiceSimulationService simulationService)
        {
            _diceSession = diceSession;
            _diceConfiguration = diceConfiguration;
            _simulationService = simulationService;
        }

        // TODO: Separate roll from reroll and reuse the same functionality for both. For example, the roll function could take a list of dice to roll and the reroll function could call it with the selected dice.
        public void RequestRoll()
        {
            // TODO: Move this to a separate function and name it "CanRollDice"
            if (_diceSession.IsRolling) return;

            // TODO: Move this to a separate function and name it "IsFirstRoll"
            bool isFirstRoll = _diceSession.ActiveDice.All(d => d.CurrentFaceIndex == -1);
            if (!isFirstRoll && _diceSession.RerollsLeft <= 0) return;

            var diceToRoll = GetDiceToRoll();
            if (!isFirstRoll && diceToRoll.Count == 0) return;

            _diceSession.IsRolling = true;

            if (!isFirstRoll)
            {
                _diceSession.RerollsLeft--;
            }

            // TODO: Move this to a separate function
            int[] targetFaceIndices = new int[diceToRoll.Count];
            DiceDefinition[] definitions = new DiceDefinition[diceToRoll.Count];

            for (int i = 0; i < diceToRoll.Count; i++)
            {
                var die = diceToRoll[i];

                // TODO: Move this line to a separate function and name it "ResetZeroedDice"
                if (die.Level == 0) die.Level = 1;

                int randomFaceIndex = die.Definition.GetRandomFaceIndex();
                die.CurrentFaceIndex = randomFaceIndex;
                die.IsSelectedForReroll = false;

                targetFaceIndices[i] = randomFaceIndex;
                definitions[i] = die.Definition;
            }

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { TargetFaceIndices = targetFaceIndices });

            // TODO: Use the SimulateRoll function instead of this as it is more generic and can be reused for other use cases
            DiceSimulationResult simulationResult = _simulationService.SimulateTrajectory(
                definitions,
                targetFaceIndices,
                GetStartPositions(diceToRoll.Count),
                GetRandomRotations(diceToRoll.Count),
                GetRandomForces(diceToRoll.Count),
                GetRandomTorques(diceToRoll.Count)
            );

            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent
            {
                SimulationResult = simulationResult,
                RolledDiceIds = diceToRoll.Select(d => d.Id).ToList()
            });
        }

        public void EndRoll()
        {
            _diceSession.IsRolling = false;
            Bus<DiceRollFinishedEvent>.Raise(new DiceRollFinishedEvent());
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;
            // TODO: Replace hardcoded with a config value
            _diceSession.RerollsLeft = 3;

            foreach (var die in _diceSession.ActiveDice)
            {
                die.CurrentFaceIndex = -1;
                die.IsSelectedForReroll = false;
            }

            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }

        public void ToggleDiceRerollSelection(string diceId)
        {
            if (_diceSession.IsRolling) return;

            // TODO: Move this to a separate function and name it "GetDiceToReroll""
            var die = _diceSession.ActiveDice.FirstOrDefault(dice => dice.Id == diceId);

            // TODO: Invert this condition to reduce nesting
            if (die != null && die.CurrentFaceIndex != -1)
            {
                die.IsSelectedForReroll = !die.IsSelectedForReroll;

                Bus<DiceRerollToggledEvent>.Raise(new DiceRerollToggledEvent
                {
                    DiceId = diceId,
                    IsSelected = die.IsSelectedForReroll
                });
            }
        }

        private List<DiceState> GetDiceToRoll()
        {
            // TODO: Move this to a separate function and name it "IsFirstRoll"
            bool isFirstRoll = _diceSession.ActiveDice.All(d => d.CurrentFaceIndex == -1);
            if (isFirstRoll) return _diceSession.ActiveDice.ToList();

            return _diceSession.ActiveDice.Where(d => d.IsSelectedForReroll).ToList();
        }

        private DiceSimulationResult SimulateRoll(List<DiceState> diceToRoll, int rollCount, int[] targetFaceIndices)
        {
            return _simulationService.SimulateTrajectory(
                diceToRoll.Select(d => d.Definition).ToArray(),
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
                Vector3 offset = new Vector3((i - (count / 2f)) * _diceConfiguration.spawnSpacing, 0, 0);

                return _diceConfiguration.spawnCenter + offset;
            });
        }

        private Vector3[] GetRandomForces(int count)
        {
            return BuildArray(count, _ =>
            {
                Vector3 randomForce = Random.onUnitSphere *
                                      Random.Range(_diceConfiguration.minForce, _diceConfiguration.maxForce);
                randomForce.y = Mathf.Abs(randomForce.y);

                return randomForce;
            });
        }

        private Vector3[] GetRandomTorques(int count)
        {
            return BuildArray(count, _ => Random.onUnitSphere * _diceConfiguration.torqueMultiplier);
        }

        private Quaternion[] GetRandomRotations(int count)
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