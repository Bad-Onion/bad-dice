using UnityEngine;

namespace _Project.Presentation.Scripts.Features.DiceSession.VisualControllers
{
    public enum DiceFaceMaterialChannel
    {
        FaceModel,
        FaceValue
    }

    public class DiceFaceMaterialBinding : MonoBehaviour
    {
        [Header("Material Target")]
        [Tooltip("Select which material channel this renderer should receive at runtime.")]
        [SerializeField] private DiceFaceMaterialChannel materialChannel = DiceFaceMaterialChannel.FaceValue;

        [Tooltip("Optional explicit renderer target. If empty, the renderer on this same GameObject is used.")]
        [SerializeField] private Renderer targetRenderer;

        public DiceFaceMaterialChannel MaterialChannel => materialChannel;

        public bool TryGetTargetRenderer(out Renderer target)
        {
            target = targetRenderer != null ? targetRenderer : GetComponent<Renderer>();
            return target != null;
        }
    }
}


