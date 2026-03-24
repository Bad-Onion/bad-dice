using System.Linq;
using UnityEngine;

namespace _Project.Domain.Features.Dice.ScriptableObjects.Definitions
{
    [CreateAssetMenu(fileName = "DiceDatabase", menuName = "Domain/Dice/DiceDatabase")]
    public class DiceDatabase : ScriptableObject
    {
        [Tooltip("Assign all available dice definitions in the game here.")]
        public DiceDefinition[] allDefinitions;

        public DiceDefinition GetDefinition(string definitionName)
        {
            return allDefinitions.FirstOrDefault(d => d.name == definitionName);
        }
    }
}