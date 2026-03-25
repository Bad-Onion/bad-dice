using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Entities;

namespace _Project.Infrastructure.Features.Combat.Damage
{
    public class DamageCalculationService : IDamageCalculationService
    {
        private readonly IDiceDamageService _diceDamageService;

        public DamageCalculationService(IDiceDamageService diceDamageService)
        {
            _diceDamageService = diceDamageService;
        }

        public int CalculateTotalDamage(IReadOnlyList<DiceState> diceStates)
        {
            if (diceStates == null || diceStates.Count == 0) return 0;

            int totalDamage = 0;

            foreach (DiceState die in diceStates)
            {
                if (die == null || die.CurrentFaceIndex < 0) continue;
                totalDamage += _diceDamageService.CalculateDamage(die);
            }

            return totalDamage;
        }

        public bool TryCalculateDiceDamage(
            IReadOnlyList<DiceState> diceStates,
            string diceId,
            out int currentValue,
            out int level,
            out int damage)
        {
            currentValue = 0;
            level = 0;
            damage = 0;
            if (diceStates == null || string.IsNullOrEmpty(diceId)) return false;

            DiceState die = diceStates.FirstOrDefault(activeDie => activeDie != null && activeDie.Dice != null && activeDie.Dice.Id == diceId);
            if (die == null || die.CurrentFaceIndex < 0) return false;

            currentValue = die.CurrentValue;
            level = die.Level;
            damage = _diceDamageService.CalculateDamage(die);
            return true;
        }
    }
}

