using System.Collections.Generic;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Domain.Entities;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    // TODO: Rename to something more descriptive and related to the DiceSession, like "DiceSessionManager"
    public class DiceManager : MonoBehaviour
    {
        private DiContainer _container;
        private DiceSession _diceSession;

        private readonly Dictionary<string, (DiceController controller, GameObject prefab)> _activeDice = new();
        private readonly Dictionary<GameObject, Queue<DiceController>> _pools = new();
        private Transform _poolRoot;

        [Inject]
        public void Construct(DiContainer container, DiceSession diceSession)
        {
            _container = container;
            _diceSession = diceSession;
        }

        private void Awake()
        {
            _poolRoot = new GameObject("DicePool").transform;
            _poolRoot.SetParent(transform);
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
            for (int i = 0; i < evt.RolledDiceIds.Count; i++)
            {
                string diceId = evt.RolledDiceIds[i];
                DiceState diceState = _diceSession.ActiveDice.Find(d => d.Id == diceId);

                if (diceState == null || diceState.Definition.visualPrefab == null) continue;

                GameObject prefab = diceState.Definition.visualPrefab;

                if (!_activeDice.TryGetValue(diceId, out var activePair))
                {
                    DiceController newController = SpawnDice(prefab);
                    activePair = (newController, prefab);
                    _activeDice[diceId] = activePair;
                }

                activePair.controller.PlayTrajectory(evt.SimulationResult.DicePaths[i]);
            }
        }

        private void HandleReset(DiceResetEvent evt)
        {
            // TODO: Rename kvp to something more descriptive like "diceEntry" or "activeDiceEntry"
            foreach (var kvp in _activeDice)
            {
                kvp.Value.controller.StopPlayback();
                DespawnDice(kvp.Value.controller, kvp.Value.prefab);
            }

            _activeDice.Clear();
        }

        // TODO: Search if it is possible to create a generic spawn/despawn method for any type of object,
        // not just dice. This would involve using a more generic type parameter and possibly a factory pattern to handle different types of objects.
        private DiceController SpawnDice(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out Queue<DiceController> pool))
            {
                pool = new Queue<DiceController>();
                _pools[prefab] = pool;
            }

            if (pool.Count > 0)
            {
                DiceController dice = pool.Dequeue();
                dice.gameObject.SetActive(true);
                return dice;
            }

            return _container.InstantiatePrefabForComponent<DiceController>(prefab, _poolRoot);
        }

        private void DespawnDice(DiceController dice, GameObject prefab)
        {
            dice.gameObject.SetActive(false);
            _pools[prefab].Enqueue(dice);
        }
    }
}