using UnityEngine;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;

namespace _Project.Infrastructure.Services
{
    public class DiceRollService : IDiceRollUseCase
    {
        private readonly DiceSession _diceSession;
        private readonly DiceConfiguration _diceConfiguration;

        public DiceRollService(DiceSession diceSession, DiceConfiguration diceConfiguration)
        {
            _diceSession = diceSession;
            _diceConfiguration = diceConfiguration;
        }

        public void RequestRoll()
        {
            if (_diceSession.IsRolling) return;

            _diceSession.IsRolling = true;
            _diceSession.TargetResult = Random.Range(1, 7);

            // Notify UI immediately
            Bus<DiceResultDecidedEvent>.Raise(new DiceResultDecidedEvent { Result = _diceSession.TargetResult });

            Vector3 randomForce = Random.onUnitSphere * Random.Range(_diceConfiguration.MinForce, _diceConfiguration.MaxForce);
            randomForce.y = Mathf.Abs(randomForce.y); // Throw slightly upwards
            Vector3 randomTorque = Random.onUnitSphere * _diceConfiguration.TorqueMultiplier;

            Bus<DiceSimulationRequestedEvent>.Raise(new DiceSimulationRequestedEvent
            {
                TargetResult = _diceSession.TargetResult,
                StartPosition = _diceConfiguration.SpawnPosition,
                StartRotation = Random.rotation,
                Force = randomForce,
                Torque = randomTorque
            });
        }

        public void ResetDice()
        {
            _diceSession.IsRolling = false;
            Bus<DiceResetEvent>.Raise(new DiceResetEvent());
        }
    }
}