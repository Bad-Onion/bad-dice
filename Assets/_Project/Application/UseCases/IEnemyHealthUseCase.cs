namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing enemy health and damage application.
    /// Handles applying damage to the current enemy during combat.
    /// </summary>
    public interface IEnemyHealthUseCase
    {
        /// <summary>
        /// Applies damage to the current enemy.
        /// </summary>
        /// <param name="amount">The amount of damage to apply.</param>
        void ApplyDamage(int amount);
    }
}

