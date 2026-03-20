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
            var mergeableIds = new List<string>();

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

        public void ExecuteAutoMerge(string targetDiceId)
        {
            if (_diceSession.IsRolling) return;

            var targetDie = _diceSession.ActiveDice.FirstOrDefault(d => d.Id == targetDiceId);
            if (targetDie == null || targetDie.CurrentFaceIndex == -1 || targetDie.Level == 0) return;

            if (!_diceSession.MergeableDiceIds.Contains(targetDie.Id)) return;

            var diceToAbsorb = _diceSession.ActiveDice
                .Where(d => d.Id != targetDie.Id && d.CurrentValue == targetDie.CurrentValue && d.Level == targetDie.Level)
                .ToList();

            if (diceToAbsorb.Count == 0) return;

            foreach (var absorbed in diceToAbsorb)
            {
                targetDie.Level += absorbed.Level;
                absorbed.Level = 0;
            }

            Bus<MergeCompletedEvent>.Raise(new MergeCompletedEvent());
        }
    }
}