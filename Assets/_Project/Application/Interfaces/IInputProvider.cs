using System;
using UnityEngine;

namespace _Project.Application.Interfaces
{
    public interface IInputProvider
    {
        event Action OnPauseAction;
        event Action OnInteract;
        Vector2 GetPointerPosition();
    }
}