using _Project.Application.Interfaces;
using _Project.Domain.Features.Dice.Enums;
using System.Collections.Generic;
using _Project.Domain.Features.Dice.DTO;
using UnityEngine;

namespace _Project.Infrastructure.Features.DiceSession.VisualServices
{
    /// <summary>
    /// Resolves face anchor positions based on configured FaceAnchorBindings.
    /// Follows Single Responsibility Principle by focusing only on anchor resolution.
    /// </summary>
    public class DiceFaceAnchorResolver : IDiceFaceAnchorResolver
    {
        private readonly Dictionary<DiceFaceDirection, Transform> _anchorsByDirection;

        public DiceFaceAnchorResolver(FaceAnchorBinding[] faceAnchors)
        {
            _anchorsByDirection = new Dictionary<DiceFaceDirection, Transform>();

            if (faceAnchors != null)
            {
                foreach (FaceAnchorBinding binding in faceAnchors)
                {
                    if (binding.anchor != null)
                    {
                        _anchorsByDirection[binding.localDirection] = binding.anchor;
                    }
                }
            }
        }

        public Transform GetAnchorForDirection(DiceFaceDirection direction)
        {
            _anchorsByDirection.TryGetValue(direction, out Transform anchor);
            return anchor;
        }
    }
}

