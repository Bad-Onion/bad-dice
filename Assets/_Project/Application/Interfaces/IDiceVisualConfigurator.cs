using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Applies visual configuration from a DiceDefinition to a dice prefab instance.
    /// </summary>
    public interface IDiceVisualConfigurator
    {
        /// <summary>
        /// Applies all visual data from the given DiceDefinition to configure the dice appearance.
        /// </summary>
        /// <param name="diceDefinition">The DiceDefinition containing visual configuration data.</param>
        void ApplyFromDefinition(DiceDefinition diceDefinition);
    }
}

