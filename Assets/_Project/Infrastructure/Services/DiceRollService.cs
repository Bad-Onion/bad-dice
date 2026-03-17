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

        public void RequestRoll()
        {
            if (_diceSession.IsRolling) return;

            var diceToRoll = GetDiceToRoll();
            if (diceToRoll.Count == 0) return;

            _diceSession.IsRolling = true;

            // TODO: Move this to a separate function
            int[] targetFaceIndices = new int[diceToRoll.Count];

            for (int i = 0; i < diceToRoll.Count; i++)
            {
                var die = diceToRoll[i];
                int randomFaceIndex = die.Definition.GetRandomFaceIndex();

                die.CurrentFaceIndex = randomFaceIndex;
                die.IsSelectedForReroll = false;

                targetFaceIndices[i] = randomFaceIndex;
            }

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { TargetFaceIndices = targetFaceIndices });

            DiceSimulationResult simulationResult = SimulateRoll(diceToRoll, diceToRoll.Count, targetFaceIndices);
            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent
                { SimulationResult = simulationResult, RolledDiceIds = diceToRoll.Select(d => d.Id).ToList() });
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;

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

            var die = _diceSession.ActiveDice.FirstOrDefault(d => d.Id == diceId);
            if (die != null)
            {
                die.IsSelectedForReroll = !die.IsSelectedForReroll;
                // TODO: Raise an event to update the UI to reflect the selection change
            }
        }

        private List<DiceState> GetDiceToRoll()
        {
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