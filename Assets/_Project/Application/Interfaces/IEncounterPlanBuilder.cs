using System.Collections.Generic;
using _Project.Domain.Features.Combat.Entities;
using _Project.Domain.Features.Combat.ScriptableObjects.Definitions;
using _Project.Domain.Features.Combat.ScriptableObjects.Settings;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Builds encounter plans for a run based on enemy databases and progression configuration.
    /// </summary>
    public interface IEncounterPlanBuilder
    {
        /// <summary>
        /// Builds the ordered list of encounters for a run.
        /// </summary>
        /// <param name="enemyDatabase">The database of available enemies.</param>
        /// <param name="progressionConfiguration">The progression configuration defining the cycle structure.</param>
        /// <returns>The generated encounter plan.</returns>
        List<EncounterPlanEntry> BuildPlan(EnemyDatabase enemyDatabase, EnemyProgressionConfiguration progressionConfiguration);
    }
}
