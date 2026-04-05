using UnityEngine;

namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for a service that provides functionality to determine what object is being targeted by a pointer (e.g., mouse cursor)
    /// based on a specified layer mask.
    /// </summary>
    public interface IPointerTargetingService
    {
        /// <summary>
        /// Tries to get the target object of type TTarget that is currently being pointed at, based on the provided layer mask.
        /// </summary>
        /// <param name="interactionLayerMask">The layer mask to use for filtering the objects that can be targeted.</param>
        /// <param name="target">The output parameter that will hold the target object if found.</param>
        /// <typeparam name="TTarget">The type of the target object to look for, which must be a Component.</typeparam>
        /// <returns>True if a target object of type TTarget is found; otherwise, false.</returns>
        bool TryGetTargetFromPointer<TTarget>(LayerMask interactionLayerMask, out TTarget target)
            where TTarget : Component;
    }
}

