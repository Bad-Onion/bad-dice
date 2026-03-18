using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities;

namespace _Project.Infrastructure.Services
{
    public class DiceMergeService : IDiceMergeUseCase
    {
        private readonly DiceSession _diceSession;

        public DiceMergeService(DiceSession diceSession)
        {
            _diceSession = diceSession;
        }

        public void EvaluateMergePossibilities()
        {
            if (_diceSession.IsMergeModeActive) return;

            var mergeableIds = new List<string>();

            // Group dice by Value and Level to find matches
            var groups = _diceSession.ActiveDice
                .Where(d => d.CurrentFaceIndex != -1 && d.Level > 0)
                .GroupBy(d => new { d.CurrentValue, d.Level })
                .Where(g => g.Count() > 1);

            foreach (var group in groups)
            {
                mergeableIds.AddRange(group.Select(d => d.Id));
            }

            _diceSession.MergeableDiceIds = mergeableIds;

            Bus<MergePossibilitiesEvaluatedEvent>.Raise(new MergePossibilitiesEvaluatedEvent
            {
                CanMerge = mergeableIds.Count > 0,
                MergeableDiceIds = mergeableIds
            });
        }

        public void ToggleMergeMode()
        {
            _diceSession.IsMergeModeActive = !_diceSession.IsMergeModeActive;
            _diceSession.MergeTargetDiceId = null;

            foreach (var die in _diceSession.ActiveDice)
            {
                die.IsSelectedForMerge = false;
            }

            Bus<MergeModeToggledEvent>.Raise(new MergeModeToggledEvent { IsActive = _diceSession.IsMergeModeActive });
            Bus<MergeSelectionUpdatedEvent>.Raise(new MergeSelectionUpdatedEvent
            {
                TargetDiceId = null,
                SelectedDiceIds = new List<string>()
            });
        }

        public void ToggleDiceMergeSelection(string diceId)
        {
            if (!_diceSession.IsMergeModeActive) return;

            var die = _diceSession.ActiveDice.FirstOrDefault(d => d.Id == diceId);
            if (die == null || die.CurrentFaceIndex == -1 || die.Level == 0) return;

            if (!_diceSession.MergeableDiceIds.Contains(die.Id)) return;

            if (string.IsNullOrEmpty(_diceSession.MergeTargetDiceId))
            {
                _diceSession.MergeTargetDiceId = die.Id;
                die.IsSelectedForMerge = true;
            }
            else
            {
                if (_diceSession.MergeTargetDiceId == die.Id)
                {
                    _diceSession.MergeTargetDiceId = null;
                    foreach (var d in _diceSession.ActiveDice) d.IsSelectedForMerge = false;
                }
                else
                {
                    var targetDie = _diceSession.ActiveDice.First(d => d.Id == _diceSession.MergeTargetDiceId);
                    if (die.CurrentValue == targetDie.CurrentValue && die.Level == targetDie.Level)
                    {
                        die.IsSelectedForMerge = !die.IsSelectedForMerge;
                    }
                }
            }

            Bus<MergeSelectionUpdatedEvent>.Raise(new MergeSelectionUpdatedEvent
            {
                TargetDiceId = _diceSession.MergeTargetDiceId,
                SelectedDiceIds = _diceSession.ActiveDice.Where(d => d.IsSelectedForMerge).Select(d => d.Id).ToList()
            });
        }

        public void SubmitMerge()
        {
            if (!_diceSession.IsMergeModeActive) return;

            var selectedDice = _diceSession.ActiveDice.Where(d => d.IsSelectedForMerge).ToList();
            bool didMerge = false;

            if (selectedDice.Count > 1 && !string.IsNullOrEmpty(_diceSession.MergeTargetDiceId))
            {
                var targetDie = selectedDice.FirstOrDefault(d => d.Id == _diceSession.MergeTargetDiceId);
                if (targetDie != null)
                {
                    var absorbedDice = selectedDice.Where(d => d.Id != targetDie.Id).ToList();

                    foreach (var absorbed in absorbedDice)
                    {
                        targetDie.Level += absorbed.Level;
                        absorbed.Level = 0;
                    }

                    didMerge = true;
                }
            }

            ToggleMergeMode();

            if (didMerge)
            {
                Bus<MergeCompletedEvent>.Raise(new MergeCompletedEvent());
            }
        }
    }
}