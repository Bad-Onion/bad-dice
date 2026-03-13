using UnityEngine;
using _Project.Application.Interfaces;

namespace _Project.Infrastructure.Adapters
{
    public class UnityTimeAdapter : ITimeService
    {
        public void SetTimeScale(float scale) => Time.timeScale = scale;
    }
}