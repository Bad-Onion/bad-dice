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
            if (_diceSession.IsRolling) return;

            int count = _diceConfiguration.DiceCount;
            if (count == 0) return; // Prevent errors if the designer empties the array

            StartDiceSession(count);

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { Results = _diceSession.TargetResults });

            DiceSimulationResult result = _simulationService.SimulateTrajectory(
                _diceSession.TargetResults,
                GetStartPositions(count),
                GetRandomRotations(count),
                GetRandomForces(count),
                GetRandomTorques(count)
            );

            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent { SimulationResult = result });
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;
            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }

        private void StartDiceSession(int count)
        {
            _diceSession.IsRolling = true;
            _diceSession.TargetResults = new int[count];

            for (int i = 0; i < count; i++)
            {
                _diceSession.TargetResults[i] = Random.Range(1, 7);
            }
        }

        private Vector3[] GetStartPositions(int count)
        {
            Vector3[] positions = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3((i - (count / 2f)) * _diceConfiguration.spawnSpacing, 0, 0);
                positions[i] = _diceConfiguration.spawnCenter + offset;
            }
            return positions;
        }

        private Vector3[] GetRandomForces(int count)
        {
            Vector3[] forces = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                Vector3 randomForce = Random.onUnitSphere * Random.Range(_diceConfiguration.minForce, _diceConfiguration.maxForce);
                randomForce.y = Mathf.Abs(randomForce.y);
                forces[i] = randomForce;
            }
            return forces;
        }

        private Vector3[] GetRandomTorques(int count)
        {
            Vector3[] torques = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                torques[i] = Random.onUnitSphere * _diceConfiguration.torqueMultiplier;
            }
            return torques;
        }

        private Quaternion[] GetRandomRotations(int count)
        {
            Quaternion[] rotations = new Quaternion[count];
            for (int i = 0; i < count; i++)
            {
                rotations[i] = Random.rotation;
            }
            return rotations;
        }
    }
}