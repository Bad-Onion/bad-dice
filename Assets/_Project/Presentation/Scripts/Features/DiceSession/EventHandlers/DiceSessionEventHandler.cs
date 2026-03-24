using System.Collections;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceInput;
using _Project.Application.Events.DiceSimulation;
using _Project.Application.Events.MergeEvents;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Dice.Simulation;
using _Project.Presentation.Scripts.Features.DiceSession.Orchestration;
using _Project.Presentation.Scripts.Features.DiceSession.Spawn;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DiceSession.EventHandlers
{
    // TODO: Check the expensive method invocations
    [RequireComponent(typeof(DicePrefabManager))]
    public class DiceSessionEventHandler : MonoBehaviour
    {
        private DiceSessionState _diceSessionState;
        private DicePrefabManager _dicePrefabManager;

        [Inject]
        public void Construct(DiceSessionState diceSessionState)
        {
            _diceSessionState = diceSessionState;
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
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent += HandleMergePossibilities;
        }

        private void OnDisable()
        {
            Bus<DicePlaybackRequestedEvent>.OnEvent -= HandlePlaybackRequested;
            Bus<DiceResetEvent>.OnEvent -= HandleReset;
            Bus<DiceRerollToggledEvent>.OnEvent -= HandleDiceSelected;
            Bus<MergePossibilitiesEvaluatedEvent>.OnEvent -= HandleMergePossibilities;
        }

        private void HandlePlaybackRequested(DicePlaybackRequestedEvent evt)
        {
            float longestPlaybackTime = GetLongestPlaybackTime(evt);

            // Unlock the session interactions after the dice finish moving
            StartCoroutine(UnlockSessionAfterDelay(longestPlaybackTime));
        }

        private float GetLongestPlaybackTime(DicePlaybackRequestedEvent evt)
        {
            float longestPlaybackTime = 0f;

            for (int i = 0; i < evt.RolledDiceIds.Count; i++)
            {
                string diceId = evt.RolledDiceIds[i];
                DiceState diceState = _diceSessionState.ActiveDice.Find(activeDice => activeDice.Dice.Id == diceId);

                if (diceState == null || diceState.Dice.Definition.visualPrefab == null) continue;

                DiceController diceController = _dicePrefabManager.GetOrSpawnDice(diceId, diceState.Dice.Definition);
                if (diceController == null) continue;
                diceController.SetSelectionVisual(false);

                DicePoseSimulationResultPath path = evt.SimulationResult.DicePaths[i];
                diceController.PlayTrajectory(path);

                float duration = GetDieAnimationDuration(path);
                if (duration > longestPlaybackTime)
                {
                    longestPlaybackTime = duration;
                }
            }

            return longestPlaybackTime;
        }

        private static float GetDieAnimationDuration(DicePoseSimulationResultPath path)
        {
            return path.Frames.Count * Time.fixedDeltaTime;
        }

        private static IEnumerator UnlockSessionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Bus<DicePlaybackCompletedEvent>.Raise(new DicePlaybackCompletedEvent());
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
