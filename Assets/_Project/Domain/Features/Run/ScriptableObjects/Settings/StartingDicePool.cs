using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;
using UnityEngine;

namespace _Project.Domain.Features.Run.ScriptableObjects.Settings
{
    [CreateAssetMenu(fileName = "StartingDicePool", menuName = "Domain/StartingDicePool")]
    public class StartingDicePool : ScriptableObject
    {
        [Tooltip("The list of dice definitions that will be given to the player at the start of a run.")]
        public DiceDefinition[] diceDefinitions;
    }
}

