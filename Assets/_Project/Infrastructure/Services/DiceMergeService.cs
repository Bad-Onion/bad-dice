using System.Collections.Generic;
using System.Linq;
using _Project.Application.Events.Core;
using _Project.Application.Events.DiceState;
using _Project.Application.Events.MergeEvents;
using _Project.Application.UseCases;
using _Project.Domain.Entities.DiceData;
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
            List<string> mergeableIds = GetMergeableDiceIds();

            _diceSessionState.MergeableDiceIds = mergeableIds;

            Bus<MergePossibilitiesEvaluatedEvent>.Raise(new MergePossibilitiesEvaluatedEvent
            {
                CanMerge = mergeableIds.Count > 0,
                MergeableDiceIds = mergeableIds
            });
        }

        public void ExecuteAutoMerge(string targetDiceId)
        {
            if (!CanMergeDice(targetDiceId, out var targetDie)) return;

            List<DiceState> diceToAbsorb = GetDicesToAbsorb(targetDie);

            if (diceToAbsorb.Count == 0) return;

            AbsorbDice(targetDie, diceToAbsorb);

            Bus<MergeCompletedEvent>.Raise(new MergeCompletedEvent());
        }

        private List<string> GetMergeableDiceIds()
        {
            return _diceSessionState.ActiveDice
                .Where(diceState => diceState.CurrentFaceIndex != -1 && diceState.Level > 0)
                .GroupBy(diceState => new { diceState.CurrentValue, diceState.Level })
                .Where(grouping => grouping.Count() > 1)
                .SelectMany(grouping => grouping.Select(diceState => diceState.Id))
                .ToList();
        }

        private bool CanMergeDice(string targetDiceId, out DiceState targetDie)
        {
            targetDie = null;

            if (_diceSessionState.IsRolling) return false;

            targetDie = _diceSessionState.ActiveDice.FirstOrDefault(diceState => diceState.Id == targetDiceId);
            if (targetDie == null || !WasDiceRolled(targetDie) || targetDie.Level == 0) return false;

            return _diceSessionState.MergeableDiceIds.Contains(targetDie.Id);
        }

        private static bool WasDiceRolled(DiceState diceState)
        {
            return diceState.CurrentFaceIndex != -1;
        }

        private List<DiceState> GetDicesToAbsorb(DiceState targetDie)
        {
            return _diceSessionState.ActiveDice
                .Where(diceState =>
                    diceState.Id != targetDie.Id &&
                    diceState.CurrentValue == targetDie.CurrentValue &&
                    diceState.Level == targetDie.Level)
                .ToList();
        }

        private static void AbsorbDice(DiceState targetDie, List<DiceState> diceToAbsorb)
        {
            foreach (var absorbedDie in diceToAbsorb)
            {
                targetDie.Level += absorbedDie.Level;
                absorbedDie.Level = 0;
            }
        }
    }
}