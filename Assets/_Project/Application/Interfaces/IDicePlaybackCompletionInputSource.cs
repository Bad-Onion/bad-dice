using System;

namespace _Project.Application.Interfaces
{
    public interface IDicePlaybackCompletionInputSource
    {
        event Action DicePlaybackCompleted;
    }
}

