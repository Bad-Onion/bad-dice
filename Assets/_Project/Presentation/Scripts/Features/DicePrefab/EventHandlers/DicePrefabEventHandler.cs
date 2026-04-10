using System;
using System.Collections;
using _Project.Application.Events.DiceInput;
using _Project.Application.Interfaces;
using _Project.Application.States.DiceSession;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;
using _Project.Domain.Features.Dice.Simulation;
using _Project.Presentation.Scripts.Features.DicePrefab.Orchestration;
using _Project.Presentation.Scripts.Features.DicePrefab.Spawn;
using UnityEngine;
using Zenject;

namespace _Project.Presentation.Scripts.Features.DicePrefab.EventHandlers
{
    [RequireComponent(typeof(DicePrefabManager))]
    public class DicePrefabEventHandler : MonoBehaviour, IDicePlaybackCompletionInputSource
    {
        private DiceSessionState _diceSessionState;
        private DiceRollState _diceRollState;
        private DiceMergeState _diceMergeState;
        private DicePrefabManager _dicePrefabManager;
        private IDiceRollUseCase _diceRollUseCase;
        private IDiceMergeUseCase _diceMergeUseCase;

        public event Action DicePlaybackCompleted;

        [Inject]
        public void Construct(
            DiceSessionState diceSessionState,
            DiceRollState diceRollState,
            DiceMergeState diceMergeState,
            IDiceRollUseCase diceRollUseCase,
            IDiceMergeUseCase diceMergeUseCase)
        {
            _diceSessionState = diceSessionState;
            _diceRollState = diceRollState;
            _diceMergeState = diceMergeState;
            _diceRollUseCase = diceRollUseCase;
            _diceMergeUseCase = diceMergeUseCase;
        }

        private void Awake()
        {
            _dicePrefabManager = GetComponent<DicePrefabManager>();
        }

        private void OnEnable()
        {
            _diceRollUseCase.DiceReset += HandleReset;
            _diceRollUseCase.DiceRerollToggled += HandleDiceSelected;
            _diceRollUseCase.DiceRollPhaseChanged += HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged += HandleMergeStateChanged;
        }

        private void OnDisable()
        {
            _diceRollUseCase.DiceReset -= HandleReset;
            _diceRollUseCase.DiceRerollToggled -= HandleDiceSelected;
            _diceRollUseCase.DiceRollPhaseChanged -= HandleDiceRollPhaseChanged;
            _diceMergeUseCase.MergeStateChanged -= HandleMergeStateChanged;
        }

        private void HandleDiceRollPhaseChanged(DiceRollPhase phase)
        {
            if (phase != DiceRollPhase.PlayingAnimation) return;

            float longestPlaybackTime = GetLongestPlaybackTime();

            // Unlock the session interactions after the dice finish moving
            StartCoroutine(UnlockSessionAfterDelay(longestPlaybackTime));
        }

        private float GetLongestPlaybackTime()
        {
            if (_diceRollState.CurrentSimulationResult.DicePaths == null) return 0f;

            float longestPlaybackTime = 0f;

            for (int i = 0; i < _diceRollState.CurrentRolledDiceIds.Count; i++)
            {
                string diceId = _diceRollState.CurrentRolledDiceIds[i];
                DiceState diceState = _diceSessionState.ActiveDice.Find(activeDice => activeDice.Dice.Id == diceId);

                if (diceState == null || diceState.Dice.Definition.visualPrefab == null) continue;

                DiceController diceController = _dicePrefabManager.GetOrSpawnDice(diceId, diceState.Dice.Definition);
                if (diceController == null) continue;
                diceController.SetSelectionVisual(false);

                DicePoseSimulationResultPath path = _diceRollState.CurrentSimulationResult.DicePaths[i];
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

        private IEnumerator UnlockSessionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            DicePlaybackCompleted?.Invoke();
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

        private void HandleMergeStateChanged(MergeState mergeState)
        {
            foreach (var activeDiceEntry in _dicePrefabManager.ActiveControllers)
            {
                bool isMergeable = _diceMergeState.MergeableDiceIds.Contains(activeDiceEntry.Key);
                activeDiceEntry.Value.SetMergeableOutline(isMergeable);
            }
        }
    }
}
