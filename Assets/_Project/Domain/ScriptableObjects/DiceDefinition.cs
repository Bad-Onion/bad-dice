using UnityEngine;

namespace _Project.Domain.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewDiceDefinition", menuName = "Domain/DiceDefinition")]
    public class DiceDefinition : ScriptableObject
    {
        [Header("Prefabs")]
        [Tooltip("The visual representation of the dice (no colliders).")]
        public GameObject visualPrefab;

        [Tooltip("The physics dummy for this specific dice (Rigidbody and Colliders only).")]
        public GameObject physicsPrefab;
    }
}