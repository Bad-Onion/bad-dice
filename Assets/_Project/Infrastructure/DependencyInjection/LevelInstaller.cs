using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Infrastructure.Services;
using _Project.Presentation.Scripts.Views;
using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Domain Entities
            Container.Bind<DiceSession>().AsSingle();

            // Services
            Container.Bind<IDiceDamageService>().To<DiceDamageService>().AsSingle();
            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();
            Container.Bind<IEncounterPreparationUseCase>().To<EncounterPreparationService>().AsSingle();

            // UI Views
            Container.Bind<PreFightView>().FromComponentInHierarchy().AsSingle();
        }
    }
}