using _Project.Domain.Features.Dice.ScriptableObjects.Definitions;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Orchestrates visual configuration of a die prefab instance
    /// Applies visual configuration from a DiceDefinition to a die prefab instance.
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

