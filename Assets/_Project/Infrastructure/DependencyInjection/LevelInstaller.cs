using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Infrastructure.Services;
using _Project.Presentation.Scripts.Views;
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
            Container.Bind<DiceSession>().AsSingle();

            // Services
            Container.Bind<IDiceDamageService>().To<DiceDamageService>().AsSingle();
            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();
            Container.Bind<IDiceMergeUseCase>().To<DiceMergeService>().AsSingle();
            Container.Bind<IEncounterPreparationUseCase>().To<EncounterPreparationService>().AsSingle();

            // UI Views
            Container.Bind<PreFightView>().FromComponentInHierarchy().AsSingle();
        }
    }
}