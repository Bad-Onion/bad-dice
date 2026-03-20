using _Project.Application.Events.Payload;
using UnityEngine;

namespace _Project.Application.Events
{
    // TODO: Move event channel to a more appropriate location
    [CreateAssetMenu(menuName = "Project/Events/Transition Event Channel", fileName = "TransitionEventChannel")]
    public class TransitionEventChannel : GenericEventChannelSO<TransitionPayload>
    {

    }
}