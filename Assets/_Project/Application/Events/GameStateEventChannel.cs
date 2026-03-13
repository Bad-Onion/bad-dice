using UnityEngine;
using System;

namespace _Project.Application.Events
{
    [CreateAssetMenu(menuName = "Project/Events/Game State Event Channel", fileName = "GameStateEventChannel")]
    public class GameStateEventChannel : GenericEventChannelSO<Type>
    {

    }
}