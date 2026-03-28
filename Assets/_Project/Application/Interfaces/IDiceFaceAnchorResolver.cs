using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Resolves and provides face anchor positions for placing face value models on a dice.
    /// </summary>
    public interface IDiceFaceAnchorResolver
    {
        /// <summary>
        /// Gets the anchor transform for a specific face direction, or null if not available.
        /// </summary>
        Transform GetAnchorForDirection(DiceFaceDirection direction);
    }
}

