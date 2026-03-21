using System.Collections;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
using _Project.Domain.Entities.Session;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Controllers
{
    [RequireComponent(typeof(DicePrefabManager))]
    public class DiceSessionEventHandler : MonoBehaviour
    {
        private DiceSessionState _diceSessionState;
        private IDiceRollUseCase _diceRollUseCase;
        private IDiceMergeUseCase _diceMergeUseCase;
        private DicePrefabManager _dicePrefabManager;

        [Inject]
        public void Construct(DiceSessionState diceSessionState, IDiceRollUseCase diceRollUseCase, IDiceMergeUseCase diceMergeUseCase)
        {
            _diceSessionState = diceSessionState;
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
        }

        private void Awake()
        {
            _dicePrefabManager = GetComponent<DicePrefabManager>();
        }

        private void OnEnable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent += HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent += HandleReset;
            Bus<DiceRerollToggledEvent>.OnEvent += HandleDiceSelected;
            Bus<DiceRollFinishedEvent>.OnEvent += HandleRollFinished;
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent += HandleMergePossibilities;
        }

        private void OnDisable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent -= HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
            Bus<DiceRerollToggledEvent>.OnEvent -= HandleDiceSelected;
            Bus<DiceRollFinishedEvent>.OnEvent -= HandleRollFinished;
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent -= HandleMergePossibilities;
        }

        private void HandlePlaybackRequested(DicePlaybackRequestedEvent evt)
        {
            // TODO: Move this to a separate function and name it "GetLongestPlaybackTime"
            float longestPlaybackTime = 0f;

            for (int i = 0; i < evt.RolledDiceIds.Count; i++)
            {
                string diceId = evt.RolledDiceIds[i];
                DiceState diceState = _diceSessionState.ActiveDice.Find(d => d.Id == diceId);

                if (diceState == null || diceState.Definition.visualPrefab == null) continue;

                DiceController diceController = _dicePrefabManager.GetOrSpawnDice(diceId, diceState.Definition.visualPrefab);
                diceController.SetSelectionVisual(false);

                var path = evt.SimulationResult.DicePaths[i];
                diceController.PlayTrajectory(path);

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
            // TODO: Use events instead of calling this function directly (see if it's possible or an anti-pattern in this case)
            _diceRollUseCase.EndRoll();
        }

        private void HandleReset(DiceResetEvent evt)
        {
            StopAllCoroutines();
            _dicePrefabManager.StopAndDespawnAllActiveDice();
        }

        private void HandleDiceSelected(DiceRerollToggledEvent evt)
        {
            if (_dicePrefabManager.TryGetActiveController(evt.DiceId, out DiceController diceController))
            {
                diceController.SetSelectionVisual(evt.IsSelected);
            }
        }

        private void HandleRollFinished(DiceRollFinishedEvent evt)
        {
            // TODO: Use events instead of calling this function directly (see if it's possible or an anti-pattern in this case)
            _diceMergeUseCase.EvaluateMergePossibilities();
        }

        private void HandleMergePossibilities(MergePossibilitiesEvaluatedEvent evt)
        {
            foreach (var activeDiceEntry in _dicePrefabManager.ActiveControllers)
            {
                bool isMergeable = evt.MergeableDiceIds.Contains(activeDiceEntry.Key);
                activeDiceEntry.Value.SetMergeableOutline(isMergeable);
            }
        }
    }
}

