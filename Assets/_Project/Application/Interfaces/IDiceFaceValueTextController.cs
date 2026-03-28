using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages text display on face value models (TextMeshPro integration).
    /// </summary>
    public interface IDiceFaceValueTextController
    {
        /// <summary>
        /// Applies text and material to all TextMeshPro components in the spawned face model.
        /// </summary>
        void ApplyFaceTexts(
            GameObject spawnedFaceModel,
            string faceValueText,
            Material faceValueMaterial);
    }
}

