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

            _diceSession.IsRolling = true;
            _diceSession.TargetResult = Random.Range(1, 7);

            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { Result = _diceSession.TargetResult });

            Vector3 randomForce = Random.onUnitSphere * Random.Range(_diceConfiguration.minForce, _diceConfiguration.maxForce);
            randomForce.y = Mathf.Abs(randomForce.y);
            Vector3 randomTorque = Random.onUnitSphere * _diceConfiguration.torqueMultiplier;
            Quaternion randomRotation = Random.rotation;

            DiceSimulationResult result = _simulationService.SimulateTrajectory(
                _diceSession.TargetResult,
                _diceConfiguration.spawnPosition,
                randomRotation,
                randomForce,
                randomTorque
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