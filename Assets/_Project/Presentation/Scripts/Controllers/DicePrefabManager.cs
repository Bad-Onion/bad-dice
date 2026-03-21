using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    public class DicePrefabManager : MonoBehaviour
    {
        private DiContainer _container;

        private readonly Dictionary<string, (DiceController controller, GameObject prefab)> _activeDice = new();
        private readonly Dictionary<GameObject, Queue<DiceController>> _pools = new();
        private Transform _poolRoot;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            _poolRoot = new GameObject("DicePool").transform;
            _poolRoot.SetParent(transform);
        }

        public DiceController GetOrSpawnDice(string diceId, GameObject prefab)
        {
            if (_activeDice.TryGetValue(diceId, out var activePair))
            {
                return activePair.controller;
            }

            DiceController newController = SpawnDice(prefab);
            newController.Initialize(diceId);

            _activeDice[diceId] = (newController, prefab);
            return newController;
        }

        public bool TryGetActiveController(string diceId, out DiceController controller)
        {
            if (_activeDice.TryGetValue(diceId, out var activePair))
            {
                controller = activePair.controller;
                return true;
            }

            controller = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, DiceController>> ActiveControllers
        {
            get
            {
                foreach (var activeDiceEntry in _activeDice)
                {
                    yield return new KeyValuePair<string, DiceController>(activeDiceEntry.Key, activeDiceEntry.Value.controller);
                }
            }
        }

        public void StopAndDespawnAllActiveDice()
        {
            foreach (var activeDiceEntry in _activeDice)
            {
                activeDiceEntry.Value.controller.StopPlayback();
                DespawnDice(activeDiceEntry.Value.controller, activeDiceEntry.Value.prefab);
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

            // TODO: Invert conditional to reduce nesting
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

