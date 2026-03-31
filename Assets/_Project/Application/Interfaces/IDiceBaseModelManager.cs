using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Manages the base model (body) of a dice, including mesh and material setup.
    /// </summary>
    public interface IDiceBaseModelManager
    {
        /// <summary>
        /// Gets the currently resolved MeshFilter, either from the spawned model or fallback.
        /// </summary>
        MeshFilter GetMeshFilter();

        /// <summary>
        /// Gets the currently resolved MeshRenderer, either from the spawned model or fallback.
        /// </summary>
        MeshRenderer GetMeshRenderer();

        /// <summary>
        /// Spawns or updates the base model prefab.
        /// </summary>
        void SetupBaseModel(GameObject baseModelPrefab);

        /// <summary>
        /// Applies the mesh to the base model.
        /// </summary>
        void ApplyMesh(Mesh baseMesh);


        /// <summary>
        /// Cleans up the spawned base model.
        /// </summary>
        void Cleanup();
    }
}

