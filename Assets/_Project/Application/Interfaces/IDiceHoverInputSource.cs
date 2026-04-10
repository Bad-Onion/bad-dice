using System;
using _Project.Application.Events.DiceInput;

namespace _Project.Application.Interfaces
{
    public interface IDiceHoverInputSource
    {
        event Action<DiceHoverChangedEvent> DiceHoverChanged;
    }
}

