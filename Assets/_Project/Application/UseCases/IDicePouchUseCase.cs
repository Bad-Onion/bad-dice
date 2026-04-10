namespace _Project.Application.UseCases
{
    /// <summary>
    /// Use case interface for managing the player's dice pouch mechanic.
    /// Handles equipping and unequipping of dice for combat use.
    /// </summary>
    public interface IDicePouchUseCase
    {
        /// <summary>
        /// Toggles the equipped status of the specified dice.
        /// Equips an unequipped dice or unequips an equipped dice.
        /// </summary>
        /// <param name="dieId">The unique identifier of the dice to toggle.</param>
        void ToggleDiceEquip(string dieId);
    }
}