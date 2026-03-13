using _Project.Application.Events.Payload;
using UnityEngine;

namespace _Project.Application.Events
{
    [CreateAssetMenu(menuName = "Project/Events/Transition Event Channel", fileName = "TransitionEventChannel")]
    public class TransitionEventChannel : GenericEventChannelSO<TransitionPayload>
    {

    }
}