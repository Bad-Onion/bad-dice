using System.Collections.Generic;
using _Project.Domain.Features.Combat.Entities;

namespace _Project.Domain.Features.Combat.Session
{
    public class CombatSessionState
    {
        public List<EncounterPlanEntry> PlannedEncounters { get; set; } = new();
        public int CurrentEncounterIndex { get; set; }
        public bool IsInitialized { get; set; }
    }
}

