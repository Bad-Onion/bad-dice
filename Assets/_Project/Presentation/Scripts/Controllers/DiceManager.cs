using System.Collections;
using System.Collections.Generic;
using _Project.Application.Events;
using _Project.Application.Events.DiceEvents;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
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
        private IDiceRollUseCase _diceRollUseCase;
        private IDiceMergeUseCase _diceMergeUseCase;

        private readonly Dictionary<string, (DiceController controller, GameObject prefab)> _activeDice = new();
        private readonly Dictionary<GameObject, Queue<DiceController>> _pools = new();
        private Transform _poolRoot;

        [Inject]
        public void Construct(DiContainer container, DiceSession diceSession, IDiceRollUseCase diceRollUseCase, IDiceMergeUseCase diceMergeUseCase)
        {
            _container = container;
            _diceSession = diceSession;
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
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
            Bus<DiceRerollToggledEvent>.OnEvent += HandleDiceSelected;
            Bus<DiceRollFinishedEvent>.OnEvent += HandleRollFinished;
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent += HandleMergePossibilities;
            Bus<MergeSelectionUpdatedEvent>.OnEvent += HandleMergeSelection;
            Bus<MergeModeToggledEvent>.OnEvent += HandleMergeModeToggled;
        }

        private void OnDisable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent -= HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
            Bus<DiceRerollToggledEvent>.OnEvent -= HandleDiceSelected;
            Bus<DiceRollFinishedEvent>.OnEvent -= HandleRollFinished;
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent -= HandleMergePossibilities;
            Bus<MergeSelectionUpdatedEvent>.OnEvent -= HandleMergeSelection;
            Bus<MergeModeToggledEvent>.OnEvent -= HandleMergeModeToggled;
        }

        private void HandlePlaybackRequested(DicePlaybackRequestedEvent evt)
        {
            float longestPlaybackTime = 0f;

            for (int i = 0; i < evt.RolledDiceIds.Count; i++)
            {
                string diceId = evt.RolledDiceIds[i];
                DiceState diceState = _diceSession.ActiveDice.Find(d => d.Id == diceId);

                if (diceState == null || diceState.Definition.visualPrefab == null) continue;

                GameObject prefab = diceState.Definition.visualPrefab;

                if (!_activeDice.TryGetValue(diceId, out var activePair))
                {
                    DiceController newController = SpawnDice(prefab);
                    newController.Initialize(diceId);

                    activePair = (newController, prefab);
                    _activeDice[diceId] = activePair;
                }

                activePair.controller.SetSelectionVisual(false);

                var path = evt.SimulationResult.DicePaths[i];
                activePair.controller.PlayTrajectory(path);

                // Calculate how long this specific die will animate
                float duration = path.Frames.Count * Time.fixedDeltaTime;
                if (duration > longestPlaybackTime)
                {
                    longestPlaybackTime = duration;
                }
            }

            // Unlock the session interactions after the dice finish moving
            StartCoroutine(UnlockSessionAfterDelay(longestPlaybackTime));
        }

        private IEnumerator UnlockSessionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _diceRollUseCase.EndRoll();
        }

        private void HandleReset(DiceResetEvent evt)
        {
            StopAllCoroutines();

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

        private void HandleDiceSelected(DiceRerollToggledEvent evt)
        {
            if (_activeDice.TryGetValue(evt.DiceId, out var activePair))
            {
                activePair.controller.SetSelectionVisual(evt.IsSelected);
            }
        }

        private void HandleRollFinished(DiceRollFinishedEvent evt)
        {
            _diceMergeUseCase.EvaluateMergePossibilities();
        }

        private void HandleMergePossibilities(MergePossibilitiesEvaluatedEvent evt)
        {
            foreach (var kvp in _activeDice)
            {
                bool isMergeable = evt.MergeableDiceIds.Contains(kvp.Key);
                kvp.Value.controller.SetMergeableOutline(isMergeable);
            }
        }

        private void HandleMergeModeToggled(MergeModeToggledEvent evt)
        {
            // Reset all selection visuals when entering/exiting
            foreach (var kvp in _activeDice)
            {
                kvp.Value.controller.SetSelectionVisual(false);
                kvp.Value.controller.SetMergeSelectionVisual(false, false);
            }
        }

        private void HandleMergeSelection(MergeSelectionUpdatedEvent evt)
        {
            foreach (var kvp in _activeDice)
            {
                bool isSelected = evt.SelectedDiceIds.Contains(kvp.Key);
                bool isTarget = kvp.Key == evt.TargetDiceId;
                kvp.Value.controller.SetMergeSelectionVisual(isSelected, isTarget);
            }
        }
    }
}