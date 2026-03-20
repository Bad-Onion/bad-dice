using _Project.Application.Events.Core;
using _Project.Application.Events.EventChannels.Payload;
using UnityEngine;

namespace _Project.Application.Events.EventChannels
{
    [CreateAssetMenu(menuName = "Project/Events/Transition Event Channel", fileName = "TransitionEventChannel")]
    public class TransitionEventChannel : GenericEventChannelSO<TransitionPayload>
    {

    }
}