using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.ScriptableObjects.Definitions;
using _Project.Domain.Features.Combat.ScriptableObjects.Settings;
using Random = UnityEngine.Random;

namespace _Project.Infrastructure.Features.Combat.Progression
{
    /// <summary>
    /// Generates encounter plans for a run.
    /// </summary>
    public class EncounterPlanBuilder : IEncounterPlanBuilder
    {
        public List<EncounterPlanEntry> BuildPlan(EnemyDatabase enemyDatabase, EnemyProgressionConfiguration progressionConfiguration)
        {
            List<EncounterPlanEntry> plannedEncounters = new();

            if (enemyDatabase == null || progressionConfiguration == null)
            {
                return plannedEncounters;
            }

            for (int cycleNumber = 1; cycleNumber <= progressionConfiguration.TotalCycles; cycleNumber++)
            {
                AddMinorEncounters(plannedEncounters, enemyDatabase.MinorEnemies, progressionConfiguration.MinorEncountersPerCycle, cycleNumber);
                AddBossEncounter(plannedEncounters, enemyDatabase, progressionConfiguration.TotalCycles, cycleNumber, progressionConfiguration.MinorEncountersPerCycle + 1);
            }

            return plannedEncounters;
        }

        private static void AddMinorEncounters(
            ICollection<EncounterPlanEntry> plannedEncounters,
            IReadOnlyList<MinorEnemyDefinition> minorEnemies,
            int minorEncountersPerCycle,
            int cycleNumber)
        {
            if (minorEnemies == null || minorEnemies.Count == 0)
            {
                return;
            }

            List<MinorEnemyDefinition> shuffledCyclePool = minorEnemies.OrderBy(_ => Random.value).ToList();

            for (int encounterIndexInCycle = 1; encounterIndexInCycle <= minorEncountersPerCycle; encounterIndexInCycle++)
            {
                MinorEnemyDefinition selectedEnemy = shuffledCyclePool.Count > 0
                    ? PopFirst(shuffledCyclePool)
                    : minorEnemies[Random.Range(0, minorEnemies.Count)];

                if (selectedEnemy == null)
                {
                    continue;
                }

                plannedEncounters.Add(CreateEncounterPlanEntry(
                    selectedEnemy.EnemyId,
                    selectedEnemy.EnemyName,
                    selectedEnemy.MaxHealth,
                    EnemyEncounterType.Minor,
                    cycleNumber,
                    encounterIndexInCycle));
            }
        }

        private static void AddBossEncounter(
            ICollection<EncounterPlanEntry> plannedEncounters,
            EnemyDatabase enemyDatabase,
            int totalCycles,
            int cycleNumber,
            int encounterIndexInCycle)
        {
            BossEnemyDefinition selectedBoss = cycleNumber == totalCycles
                ? enemyDatabase.FinalBoss
                : GetRandomBoss(enemyDatabase);

            if (selectedBoss == null)
            {
                return;
            }

            plannedEncounters.Add(CreateEncounterPlanEntry(
                selectedBoss.EnemyId,
                selectedBoss.EnemyName,
                selectedBoss.MaxHealth,
                cycleNumber == totalCycles ? EnemyEncounterType.FinalBoss : EnemyEncounterType.Boss,
                cycleNumber,
                encounterIndexInCycle));
        }

        private static EncounterPlanEntry CreateEncounterPlanEntry(
            string enemyId,
            string enemyName,
            int maxHealth,
            EnemyEncounterType encounterType,
            int cycleNumber,
            int encounterIndexInCycle)
        {
            return new EncounterPlanEntry
            {
                EnemyId = enemyId,
                EnemyName = enemyName,
                MaxHealth = maxHealth,
                EncounterType = encounterType,
                CycleNumber = cycleNumber,
                EncounterIndexInCycle = encounterIndexInCycle
            };
        }

        private static MinorEnemyDefinition PopFirst(List<MinorEnemyDefinition> entries)
        {
            MinorEnemyDefinition selectedEnemy = entries[0];
            entries.RemoveAt(0);
            return selectedEnemy;
        }

        private static BossEnemyDefinition GetRandomBoss(EnemyDatabase enemyDatabase)
        {
            if (enemyDatabase.Bosses == null || enemyDatabase.Bosses.Count == 0)
            {
                return null;
            }

            List<BossEnemyDefinition> candidateBosses = enemyDatabase.Bosses
                .Where(boss => boss != null && boss != enemyDatabase.FinalBoss)
                .ToList();

            if (candidateBosses.Count == 0)
            {
                candidateBosses = enemyDatabase.Bosses.Where(boss => boss != null).ToList();
            }

            if (candidateBosses.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, candidateBosses.Count);
            return candidateBosses[randomIndex];
        }
    }
}
