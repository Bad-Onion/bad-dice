using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.States.DiceSession;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.Session;

namespace _Project.Infrastructure.Features.DiceSession.UseCases
{
    public class DiceMergeService : IDiceMergeUseCase
    {
        private readonly DiceSessionState _diceSessionState;
        private readonly DiceRollState _diceRollState;
        private readonly DiceMergeState _diceMergeState;

        public event Action<MergeState> MergeStateChanged;

        public DiceMergeService(
            DiceSessionState diceSessionState,
            DiceRollState diceRollState,
            DiceMergeState diceMergeState)
        {
            _diceSessionState = diceSessionState;
            _diceRollState = diceRollState;
            _diceMergeState = diceMergeState;
        }

        public void EvaluateMergePossibilities()
        {
            List<string> mergeableIds = GetMergeableDiceIds();

            _diceMergeState.MergeableDiceIds = mergeableIds;
            _diceMergeState.MergeState = mergeableIds.Count > 0 ? MergeState.Available : MergeState.None;

            MergeStateChanged?.Invoke(_diceMergeState.MergeState);
        }

        public void ExecuteMerge(string targetDieId)
        {
            if (!CanMergeDice(targetDieId, out var targetDie)) return;

            List<DiceState> diceToAbsorb = GetDicesToAbsorb(targetDie);

            if (diceToAbsorb.Count == 0) return;

            AbsorbDice(targetDie, diceToAbsorb);
            _diceMergeState.MergeState = MergeState.Applied;

            MergeStateChanged?.Invoke(_diceMergeState.MergeState);
        }

        private List<string> GetMergeableDiceIds()
        {
            return _diceSessionState.ActiveDice
                .Where(diceState => diceState.CurrentFaceIndex != -1 && diceState.Level > 0)
                .GroupBy(diceState => new { diceState.CurrentValue, diceState.Level })
                .Where(grouping => grouping.Count() > 1)
                .SelectMany(grouping => grouping.Select(diceState => diceState.Dice.Id))
                .ToList();
        }

        private bool CanMergeDice(string targetDiceId, out DiceState targetDie)
        {
            targetDie = null;

            if (_diceRollState.IsRolling) return false;

            targetDie = _diceSessionState.ActiveDice.FirstOrDefault(diceState => diceState.Dice.Id == targetDiceId);
            if (targetDie == null || !WasDiceRolled(targetDie) || targetDie.Level == 0) return false;

            return _diceMergeState.MergeableDiceIds.Contains(targetDie.Dice.Id);
        }

        private static bool WasDiceRolled(DiceState diceState)
        {
            return diceState.CurrentFaceIndex != -1;
        }

        private List<DiceState> GetDicesToAbsorb(DiceState targetDie)
        {
            return _diceSessionState.ActiveDice
                .Where(diceState =>
                    diceState.Dice.Id != targetDie.Dice.Id &&
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