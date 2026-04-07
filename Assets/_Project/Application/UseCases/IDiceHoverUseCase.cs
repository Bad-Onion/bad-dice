using System;
using _Project.Application.Events.DiceState;

namespace _Project.Application.UseCases
{
    public interface IDiceHoverUseCase
    {
        event Action<DiceHoverDetailsUpdatedEvent> DiceHoverDetailsUpdated;
    }
}

