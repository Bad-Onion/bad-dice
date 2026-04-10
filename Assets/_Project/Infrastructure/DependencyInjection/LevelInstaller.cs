using _Project.Application.Commands;
using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Infrastructure.Features.Combat.Damage;
using _Project.Infrastructure.Features.DiceSession.Orchestration;
using _Project.Infrastructure.Features.DiceSession.UseCases;
using _Project.Infrastructure.Features.Inventory;
using _Project.Infrastructure.Shared;
using _Project.Presentation.Scripts.Features.DicePrefab.EventHandlers;
using _Project.Presentation.Scripts.Features.DicePrefab.Input;
using _Project.Presentation.Scripts.Features.DiceSession.Presenters;
using _Project.Presentation.Scripts.Features.Inventory.Views;
using UnityEngine;
using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    /// <summary>
    /// The LevelInstaller is responsible for setting up all dependency bindings specific to a level or scene in game. This
    /// includes services, use cases, and any references that are unique to the level's functionality.
    /// </summary>
    public class LevelInstaller : MonoInstaller
    {
        [Header("Level References")]
        [Tooltip("Assign the specific camera for this level.")]
        [SerializeField] private Camera levelCamera;

        public override void InstallBindings()
        {
            // Level References
            Container.BindInstance(levelCamera).AsSingle();

            // Core Services
            Container.Bind<IDiceDamageService>().To<DiceDamageService>().AsSingle();
            Container.Bind<IDamageCalculationService>().To<DamageCalculationService>().AsSingle();
            Container.Bind<IDealDamageUseCase>().To<DealDamageService>().AsSingle();
            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();
            Container.Bind<IDiceMergeUseCase>().To<DiceMergeService>().AsSingle();
            Container.Bind<IDicePouchUseCase>().To<DicePouchService>().AsSingle();
            Container.Bind<IPointerTargetingService>().To<PointerTargetingService>().AsSingle();

            // Input Sources \(Scene Components\)
            Container.Bind<IDiceHoverInputSource>().To<DiceSelectionHandler>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IDicePlaybackCompletionInputSource>().To<DicePrefabEventHandler>().FromComponentInHierarchy().AsSingle();

            // Interaction Services
            Container.BindInterfacesAndSelfTo<DiceHoverService>().AsSingle();

            // Presenters
            Container.Bind<DiceSelectionPresenter>().AsSingle();
            Container.Bind<DiceSessionEncounterViewPresenter>().AsSingle();
            Container.Bind<DiceSessionRollStatePresenter>().AsSingle();
            Container.Bind<DiceSessionCombatFeedbackPresenter>().AsSingle();
            Container.Bind<DiceSessionCommandPresenter>().AsSingle();
            Container.Bind<DiceSessionEventPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<DiceSessionPresenterFacade>().AsSingle();

            // Commands
            Container.Bind<StartEncounterCommand>().AsTransient();
            Container.Bind<DealDamageCommand>().AsTransient();
            Container.Bind<RequestDiceRollCommand>().AsTransient();
            Container.Bind<ResetDiceCommand>().AsTransient();

            // Command Factories
            Container.BindFactory<string, ToggleDiceRerollSelectionCommand, ToggleDiceRerollSelectionCommand.Factory>().AsSingle();
            Container.BindFactory<string, ExecuteDiceMergeCommand, ExecuteDiceMergeCommand.Factory>().AsSingle();

            // Coordinators
            Container.BindInterfacesTo<DiceSessionFlowCoordinator>().AsSingle();

            // UI Views
            Container.Bind<DicePouchSelectorView>().FromComponentInHierarchy().AsSingle();
        }
    }
}