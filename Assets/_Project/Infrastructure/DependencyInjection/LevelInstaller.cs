using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Features.Dice.Session;
using _Project.Infrastructure.Features.Combat.Damage;
using _Project.Infrastructure.Features.DiceSession.Orchestration;
using _Project.Infrastructure.Features.DiceSession.UseCases;
using _Project.Infrastructure.Features.Inventory;
using _Project.Infrastructure.Shared;
using _Project.Presentation.Scripts.Features.DiceSession.Input;
using _Project.Presentation.Scripts.Features.Inventory.Views;
using UnityEngine;
using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    public class LevelInstaller : MonoInstaller
    {
        [Header("Level References")]
        [Tooltip("Assign the specific camera for this level.")]
        [SerializeField] private Camera levelCamera;

        public override void InstallBindings()
        {
            // Level References
            Container.BindInstance(levelCamera).AsSingle();

            // Domain Entities
            Container.Bind<DiceSessionState>().AsSingle();

            // Services
            Container.Bind<IDiceDamageService>().To<DiceDamageService>().AsSingle();
            Container.Bind<IDealDamageUseCase>().To<DealDamageService>().AsSingle();
            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();
            Container.Bind<IDiceMergeUseCase>().To<DiceMergeService>().AsSingle();
            Container.Bind<IDicePouchUseCase>().To<DicePouchService>().AsSingle();
            Container.Bind<IPointerTargetingService>().To<PointerTargetingService>().AsSingle();
            Container.Bind<DiceSelectionPresenter>().AsSingle();
            Container.BindInterfacesTo<DiceSessionFlowCoordinator>().AsSingle();

            // UI Views
            Container.Bind<DicePouchSelectorView>().FromComponentInHierarchy().AsSingle();
        }
    }
}