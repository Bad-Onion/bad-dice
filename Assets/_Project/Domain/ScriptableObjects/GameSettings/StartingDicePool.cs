using _Project.Domain.ScriptableObjects.DiceDefinitions;
using UnityEngine;

namespace _Project.Domain.ScriptableObjects.GameSettings
{
    [CreateAssetMenu(fileName = "StartingDicePool", menuName = "Domain/StartingDicePool")]
    public class StartingDicePool : ScriptableObject
    {
        [Tooltip("The list of dice definitions that will be given to the player at the start of a run.")]
        public DiceDefinition[] diceDefinitions;
    }
}

