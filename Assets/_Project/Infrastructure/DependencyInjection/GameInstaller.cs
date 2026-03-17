using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using _Project.Infrastructure.Services;
using Zenject;
using UnityEngine;

namespace _Project.Infrastructure.DependencyInjection
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Game Configuration")]
        [Tooltip("Assign the main game configuration containing the default level data.")]
        [SerializeField] private GameConfiguration gameConfiguration;

        public override void InstallBindings()
        {
            // Configuration Data
            Container.BindInstance(gameConfiguration).AsSingle();

            // Entities
            Container.Bind<GameSession>().AsSingle();

            // Services
            Container.BindInterfacesTo<GameFlowService>().AsSingle();
        }
    }
}