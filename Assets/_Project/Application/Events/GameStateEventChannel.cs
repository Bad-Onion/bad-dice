using UnityEngine;
using System;

namespace _Project.Application.Events
{
    // TODO: Move event channel to a more appropriate location
    [CreateAssetMenu(menuName = "Project/Events/Game State Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannel : GenericEventChannelSO<Type>
    {

    }
}