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

        public DiceRollService(DiceSession diceSession, DiceConfiguration diceConfiguration, IDiceSimulationService simulationService)
        {
            _diceSession = diceSession;
            _diceConfiguration = diceConfiguration;
            _simulationService = simulationService;
        }

        public void RequestRoll()
        {
            if (!CanStartRoll(out int count)) return;

            StartDiceSession(count);
            PublishResultDecided();

            DiceSimulationResult simulationResult = SimulateRoll(count);
            PublishPlaybackRequested(simulationResult);
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;
            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }

        private bool CanStartRoll(out int count)
        {
            count = _diceConfiguration.DiceCount;
            return !_diceSession.IsRolling && count != 0;
        }

        private void StartDiceSession(int count)
        {
            _diceSession.IsRolling = true;
            _diceSession.TargetResults = BuildArray(count, _ => Random.Range(1, 7));
        }

        private void PublishResultDecided()
        {
            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { Results = _diceSession.TargetResults });
        }

        private DiceSimulationResult SimulateRoll(int count)
        {
            return _simulationService.SimulateTrajectory(
                _diceSession.TargetResults,
                GetStartPositions(count),
                GetRandomRotations(count),
                GetRandomForces(count),
                GetRandomTorques(count)
            );
        }

        private void PublishPlaybackRequested(DiceSimulationResult simulationResult)
        {
            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent { SimulationResult = simulationResult });
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
                Vector3 randomForce = Random.onUnitSphere * Random.Range(_diceConfiguration.minForce, _diceConfiguration.maxForce);
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