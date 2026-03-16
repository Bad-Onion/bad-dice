using System.Collections.Generic;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceManager : MonoBehaviour
    {
        private DiceController.Pool _dicePool;
        private readonly List<DiceController> _activeDice = new List<DiceController>();

        [Inject]
        public void Construct(DiceController.Pool dicePool)
        {
            _dicePool = dicePool;
        }

        private void OnEnable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent += HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent += HandleReset;
        }

        private void OnDisable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent -= HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
        }

        private void HandlePlaybackRequested(DicePlaybackRequestedEvent evt)
        {
            HandleReset(new DiceResetEvent());

            foreach (var path in evt.SimulationResult.DicePaths)
            {
                DiceController dice = _dicePool.Spawn();
                _activeDice.Add(dice);
                dice.PlayTrajectory(path);
            }
        }

        private void HandleReset(DiceResetEvent evt)
        {
            foreach (var dice in _activeDice)
            {
                _dicePool.Despawn(dice);
            }
            _activeDice.Clear();
        }
    }
}