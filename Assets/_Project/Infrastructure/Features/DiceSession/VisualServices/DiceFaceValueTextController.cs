using _Project.Application.Interfaces;
using TMPro;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Manages TextMeshPro text display on face value models.
    /// Follows Single Responsibility Principle by focusing only on text management.
    /// </summary>
    public class DiceFaceValueTextController : IDiceFaceValueTextController
    {
        public void ApplyFaceTexts(
            GameObject spawnedFaceModel,
            string faceValueText,
            Material faceValueMaterial)
        {
            TMP_Text[] texts = spawnedFaceModel.GetComponentsInChildren<TMP_Text>(true);
            if (texts.Length == 0) return;

            for (int index = 0; index < texts.Length; index++)
            {
                if (faceValueText != null)
                {
                    texts[index].text = faceValueText;
                }

                if (faceValueMaterial != null)
                {
                    texts[index].fontSharedMaterial = faceValueMaterial;
                }
            }
        }
    }
}

