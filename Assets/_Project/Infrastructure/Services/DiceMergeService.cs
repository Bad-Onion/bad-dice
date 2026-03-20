using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities.Session;

namespace _Project.Infrastructure.Services
{
    public class DiceMergeService : IDiceMergeUseCase
    {
        private readonly DiceSessionState _diceSessionState;

        public DiceMergeService(DiceSessionState diceSessionState)
        {
            _diceSessionState = diceSessionState;
        }

        public void EvaluateMergePossibilities()
        {
            var mergeableIds = new List<string>();

            // TODO: Move to another function and name to "GetMergeableDiceIds"
            var mergeableGroups = _diceSessionState.ActiveDice
                .Where(diceState => diceState.CurrentFaceIndex != -1 && diceState.Level > 0)
                .GroupBy(diceState => new { diceState.CurrentValue, diceState.Level })
                .Where(grouping => grouping.Count() > 1);

            foreach (var group in mergeableGroups)
            {
                mergeableIds.AddRange(group.Select(diceState => diceState.Id));
            }

            _diceSessionState.MergeableDiceIds = mergeableIds;

            Bus<MergePossibilitiesEvaluatedEvent>.Raise(new MergePossibilitiesEvaluatedEvent
            {
                CanMerge = mergeableIds.Count > 0,
                MergeableDiceIds = mergeableIds
            });
        }

        public void ExecuteAutoMerge(string targetDiceId)
        {
            // TODO: Move conditions to another function and name to "CanMergeDice"
            if (_diceSessionState.IsRolling) return;

            var targetDie = _diceSessionState.ActiveDice.FirstOrDefault(d => d.Id == targetDiceId);
            if (targetDie == null || targetDie.CurrentFaceIndex == -1 || targetDie.Level == 0) return;

            if (!_diceSessionState.MergeableDiceIds.Contains(targetDie.Id)) return;

            // TODO: Move to another function and name to "GetDicesToAbsorb"
            var diceToAbsorb = _diceSessionState.ActiveDice
                .Where(d => d.Id != targetDie.Id && d.CurrentValue == targetDie.CurrentValue && d.Level == targetDie.Level)
                .ToList();

            if (diceToAbsorb.Count == 0) return;

            // TODO: Move to another function and name to "AbsorbDice"
            foreach (var absorbed in diceToAbsorb)
            {
                targetDie.Level += absorbed.Level;
                absorbed.Level = 0;
            }

            Bus<MergeCompletedEvent>.Raise(new MergeCompletedEvent());
        }
    }
}