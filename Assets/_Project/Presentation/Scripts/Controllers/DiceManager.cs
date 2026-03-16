using System.Collections.Generic;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Domain.ScriptableObjects;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DiceManager : MonoBehaviour
    {
        private DiceConfiguration _config;
        private DiContainer _container;

        private readonly List<(DiceController controller, GameObject prefab)> _activeDice = new();
        private readonly Dictionary<GameObject, Queue<DiceController>> _pools = new();
        private Transform _poolRoot;

        [Inject]
        public void Construct(DiceConfiguration config, DiContainer container)
        {
            _config = config;
            _container = container;
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
            HandleReset(new DiceResetEvent());

            for (int i = 0; i < evt.SimulationResult.DicePaths.Count; i++)
            {
                if (i >= _config.diceDefinitions.Length) break;

                DiceDefinition definition = _config.diceDefinitions[i];
                if (definition == null || definition.visualPrefab == null) continue;

                GameObject prefab = definition.visualPrefab;

                DiceController dice = SpawnDice(prefab);
                _activeDice.Add((dice, prefab));

                dice.PlayTrajectory(evt.SimulationResult.DicePaths[i]);
            }
        }

        private void HandleReset(DiceResetEvent evt)
        {
            foreach (var active in _activeDice)
            {
                active.controller.StopPlayback();
                DespawnDice(active.controller, active.prefab);
            }
            _activeDice.Clear();
        }

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

            DiceController newDice = _container.InstantiatePrefabForComponent<DiceController>(prefab, _poolRoot);
            return newDice;
        }

        private void DespawnDice(DiceController dice, GameObject prefab)
        {
            dice.gameObject.SetActive(false);
            _pools[prefab].Enqueue(dice);
        }
    }
}