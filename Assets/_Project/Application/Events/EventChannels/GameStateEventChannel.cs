using System;
using _Project.Application.Events.Core;
using UnityEngine;

namespace _Project.Application.Events.EventChannels
{
    [CreateAssetMenu(menuName = "Project/Events/Game State Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannel : GenericEventChannelSO<Type>
    {

    }
}