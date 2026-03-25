using System.Collections.Generic;
using System.Linq;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.Enums;
using _Project.Domain.Features.Combat.Session;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using _Project.Domain.Features.Run.DTO;
using _Project.Domain.Features.Run.Session;

namespace _Project.Infrastructure.Features.Run.Persistence
{
    public static class RunDataConverter
    {
        public static PlayerRunSaveData ToSaveData(PlayerRunState runState, CombatSessionState combatSessionState)
        {
            var saveData = new PlayerRunSaveData
            {
                maxEquippedDice = runState.MaxEquippedDice,
                rerollsPerTurn = runState.RerollsPerTurn,
                turnsPerFight = runState.TurnsPerFight,
                combatProgression = ToCombatProgressionSaveData(combatSessionState)
            };

            foreach (var ownedDice in runState.DiceInventory)
            {
                saveData.diceInventory.Add(new OwnedDiceSaveData
                {
                    id = ownedDice.Dice.Id,
                    definitionName = ownedDice.Dice.Definition.name,
                    isEquipped = ownedDice.IsEquipped
                });
            }

            return saveData;
        }

        public static PlayerRunState ToRunState(PlayerRunSaveData saveData, DiceDatabase diceDatabase)
        {
            var runState = new PlayerRunState
            {
                MaxEquippedDice = saveData.maxEquippedDice,
                RerollsPerTurn = saveData.rerollsPerTurn,
                TurnsPerFight = saveData.turnsPerFight
            };

            foreach (var savedDice in saveData.diceInventory)
            {
                DiceDefinition definition = diceDatabase.GetDefinition(savedDice.definitionName);
                if (definition == null) continue;

                runState.DiceInventory.Add(new OwnedDiceData
                {
                    Dice = new DiceData
                    {
                        Id = savedDice.id,
                        Definition = definition
                    },
                    IsEquipped = savedDice.isEquipped
                });
            }

            return runState;
        }

        public static void ApplyCombatProgression(PlayerRunSaveData saveData, CombatSessionState combatSessionState)
        {
            if (combatSessionState == null) return;

            combatSessionState.PlannedEncounters.Clear();
            combatSessionState.CurrentEncounterIndex = 0;
            combatSessionState.IsInitialized = false;

            if (saveData?.combatProgression == null) return;

            List<EncounterPlanEntry> plannedEntries = saveData.combatProgression.plannedEncounters
                .Select(ToEncounterPlanEntry)
                .Where(entry => entry != null)
                .ToList();

            if (plannedEntries.Count == 0) return;

            combatSessionState.PlannedEncounters.AddRange(plannedEntries);
            combatSessionState.CurrentEncounterIndex = ClampEncounterIndex(
                saveData.combatProgression.currentEncounterIndex,
                plannedEntries.Count);
            combatSessionState.IsInitialized = true;
        }

        private static CombatProgressionSaveData ToCombatProgressionSaveData(CombatSessionState combatSessionState)
        {
            if (combatSessionState == null)
            {
                return new CombatProgressionSaveData();
            }

            int safeEncounterIndex = ClampEncounterIndex(
                combatSessionState.CurrentEncounterIndex,
                combatSessionState.PlannedEncounters.Count);

            EncounterPlanEntry currentEncounter = combatSessionState.PlannedEncounters.Count > 0
                ? combatSessionState.PlannedEncounters[safeEncounterIndex]
                : null;

            return new CombatProgressionSaveData
            {
                plannedEncounters = combatSessionState.PlannedEncounters
                    .Select(ToEncounterPlanSaveData)
                    .Where(entry => entry != null)
                    .ToList(),
                currentEncounterIndex = safeEncounterIndex,
                currentCycleNumber = currentEncounter?.CycleNumber ?? 1,
                currentEnemyId = currentEncounter?.EnemyId
            };
        }

        private static EncounterPlanSaveData ToEncounterPlanSaveData(EncounterPlanEntry encounterPlanEntry)
        {
            if (encounterPlanEntry == null) return null;

            return new EncounterPlanSaveData
            {
                enemyId = encounterPlanEntry.EnemyId,
                enemyName = encounterPlanEntry.EnemyName,
                maxHealth = encounterPlanEntry.MaxHealth,
                encounterType = (int)encounterPlanEntry.EncounterType,
                cycleNumber = encounterPlanEntry.CycleNumber,
                encounterIndexInCycle = encounterPlanEntry.EncounterIndexInCycle
            };
        }

        private static EncounterPlanEntry ToEncounterPlanEntry(EncounterPlanSaveData saveData)
        {
            if (saveData == null) return null;

            return new EncounterPlanEntry
            {
                EnemyId = saveData.enemyId,
                EnemyName = saveData.enemyName,
                MaxHealth = saveData.maxHealth,
                EncounterType = (EnemyEncounterType)saveData.encounterType,
                CycleNumber = saveData.cycleNumber,
                EncounterIndexInCycle = saveData.encounterIndexInCycle
            };
        }

        private static int ClampEncounterIndex(int index, int plannedCount)
        {
            if (plannedCount <= 0) return 0;
            if (index < 0) return 0;
            if (index >= plannedCount) return plannedCount - 1;

            return index;
        }
    }
}