namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for handling damage dealing mechanics.
    /// Manages the process of calculating and applying damage based on the damage providers (e.g., dice rolls, consumable items, pact passives).
    /// </summary>
    public interface IDealDamageUseCase
    {
        /// <summary>
        /// Executes the damage dealing action using the results of the damage providers (e.g., dice rolls, consumable items, pact passives).
        /// Calculates total damage from damage providers and applies it to the current enemy.
        /// </summary>
        void DealCurrentDamage();
    }
}

