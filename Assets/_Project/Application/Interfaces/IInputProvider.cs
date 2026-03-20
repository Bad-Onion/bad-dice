using System;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IInputProvider
    {
        event Action OnPauseAction;
        event Action OnInteract;
        event Action OnHoldInteract;
        Vector2 GetPointerPosition();
    }
}