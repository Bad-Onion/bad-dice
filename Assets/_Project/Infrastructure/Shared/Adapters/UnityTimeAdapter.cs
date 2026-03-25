using _Project.Application.Interfaces;
using UnityEngine;

namespace _Project.Infrastructure.Shared.Adapters
{
    public class UnityTimeAdapter : ITimeService
    {
        public void SetTimeScale(float scale) => Time.timeScale = scale;
    }
}