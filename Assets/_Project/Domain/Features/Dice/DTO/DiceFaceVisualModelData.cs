using System;
using _Project.Domain.Features.Dice.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Domain.Features.Dice.DTO
{
    [Serializable]
    public struct DiceFaceVisualModelData
    {
        [Header("Visuals")]
        [Tooltip("The local direction where this face visual should be attached.")]
        public DiceFaceDirection localDirection;

        [Tooltip("Value model prefab exported from Blender for this face value.")]
        [FormerlySerializedAs("modelPrefab")]
        public GameObject faceValuePrefab;

        [Tooltip("Material applied to the model surface channel of this face visual.")]
        [FormerlySerializedAs("baseMaterial")]
        public Material faceModelMaterial;

        [Tooltip("Material applied to the value channel (number/symbol) of this face visual.")]
        public Material faceValueMaterial;

        [Header("Transform Customization")]
        [Tooltip("Local position offset for this face model. Leave at (0,0,0) for default anchor position.")]
        public Vector3 localPositionOffset;

        [Tooltip("Local rotation (Euler angles) for this face model. Leave at (0,0,0) for identity rotation.")]
        public Vector3 localRotationEuler;

        [Header("Text Configuration")]
        [Tooltip("If enabled and the prefab contains TMP text components, runtime uses the gameplay face value as text.")]
        public bool useGameplayFaceValueAsText;

        [Tooltip("Optional text override used only when Use Gameplay Face Value As Text is disabled.")]
        public string customFaceValueText;
    }
}