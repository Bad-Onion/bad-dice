using _Project.Domain.Features.Dice.DTO;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages the spawning and configuration of face value models for a dice.
    /// </summary>
    public interface IDiceFaceModelManager
    {
        /// <summary>
        /// Spawns and configures all face models according to the visual configuration.
        /// </summary>
        void SetupFaceModels(
            DiceFaceVisualModelData[] faceModels,
            DiceFaceData[] gameplayFaces);

        /// <summary>
        /// Cleans up all spawned face models.
        /// </summary>
        void Cleanup();
    }
}

