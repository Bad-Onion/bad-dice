using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Entities;
using _Project.Domain.Features.Run.ScriptableObjects.Settings;
using _Project.Domain.Features.Run.Session;

namespace _Project.Infrastructure.Features.Run.Persistence
{
    public class RunStateBuilder : IRunStateBuilder
    {
        public void BuildFromExisting(PlayerRunState targetState, PlayerRunState sourceState,
            RunDefinitions runDefinitions)
        {
            ValidateArguments(targetState, runDefinitions);

            if (sourceState == null)
            {
                BuildNew(targetState, runDefinitions);
                return;
            }

            targetState.DiceInventory = sourceState.DiceInventory ?? new List<OwnedDiceData>();
            targetState.MaxEquippedDice = ResolveOrDefault(sourceState.MaxEquippedDice, runDefinitions.maxEquippedDice);
            targetState.RerollsPerTurn = ResolveOrDefault(sourceState.RerollsPerTurn, runDefinitions.rerollsPerTurn);
            targetState.TurnsPerFight = ResolveOrDefault(sourceState.TurnsPerFight, runDefinitions.turnsPerFight);
        }

        public void BuildNew(PlayerRunState targetState, RunDefinitions runDefinitions)
        {
            ValidateArguments(targetState, runDefinitions);

            targetState.DiceInventory = CreateStartingInventory(runDefinitions);
            targetState.MaxEquippedDice = runDefinitions.maxEquippedDice;
            targetState.RerollsPerTurn = runDefinitions.rerollsPerTurn;
            targetState.TurnsPerFight = runDefinitions.turnsPerFight;
        }

        private static void ValidateArguments(PlayerRunState targetState, RunDefinitions runDefinitions)
        {
            if (targetState == null)
            {
                throw new ArgumentNullException(nameof(targetState));
            }

            if (runDefinitions == null)
            {
                throw new ArgumentNullException(nameof(runDefinitions));
            }
        }

        private static int ResolveOrDefault(int value, int fallback)
        {
            return value > 0 ? value : fallback;
        }

        private static List<OwnedDiceData> CreateStartingInventory(RunDefinitions runDefinitions)
        {
            List<OwnedDiceData> diceInventory = new();

            if (runDefinitions.startingDicePool == null || runDefinitions.startingDicePool.diceDefinitions == null)
            {
                return diceInventory;
            }

            return runDefinitions.startingDicePool.diceDefinitions
                .Select(definition => new OwnedDiceData
                {
                    Dice = new DiceData
                    {
                        Id = Guid.NewGuid().ToString(),
                        Definition = definition
                    },
                    IsEquipped = true
                })
                .ToList();
        }
    }
}