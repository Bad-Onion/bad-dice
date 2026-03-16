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
            _diceSession.IsRolling = true;
            _diceSession.TargetResults = new int[count];

            Vector3[] startPos = new Vector3[count];
            Quaternion[] startRot = new Quaternion[count];
            Vector3[] forces = new Vector3[count];
            Vector3[] torques = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                _diceSession.TargetResults[i] = Random.Range(1, 7);

                Vector3 offset = new Vector3((i - (count / 2f)) * _diceConfiguration.SpawnSpacing, 0, 0);
                startPos[i] = _diceConfiguration.SpawnCenter + offset;
                startRot[i] = Random.rotation;

                Vector3 randomForce = Random.onUnitSphere * Random.Range(_diceConfiguration.MinForce, _diceConfiguration.MaxForce);
                randomForce.y = Mathf.Abs(randomForce.y);
                forces[i] = randomForce;
                torques[i] = Random.onUnitSphere * _diceConfiguration.TorqueMultiplier;
            }

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { Results = _diceSession.TargetResults });

            DiceSimulationResult result = _simulationService.SimulateTrajectory(
                _diceSession.TargetResults, startPos, startRot, forces, torques
            );

            Bus<DicePlaybackRequestedEvent>.Raise(new DicePlaybackRequestedEvent { SimulationResult = result });
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;
            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }
    }
}