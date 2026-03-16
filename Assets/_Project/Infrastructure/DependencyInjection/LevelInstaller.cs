using _Project.Application.Interfaces;
using _Project.Application.UseCases;
using _Project.Domain.Entities;
using _Project.Domain.ScriptableObjects;
using _Project.Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private DiceConfiguration diceConfiguration;

        public override void InstallBindings()
        {
            Container.BindInstance(diceConfiguration).AsSingle();
            Container.Bind<DiceSession>().AsSingle();

            Container.Bind<IDiceSimulationService>().To<DiceSimulationService>().AsSingle();
            Container.Bind<IDiceRollUseCase>().To<DiceRollService>().AsSingle();
        }
    }
}