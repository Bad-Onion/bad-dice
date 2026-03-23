using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IPointerTargetingService
    {
        bool TryGetTargetFromPointer<TTarget>(LayerMask interactionLayerMask, out TTarget target)
            where TTarget : Component;
    }
}

