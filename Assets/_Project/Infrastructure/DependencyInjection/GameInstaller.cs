using Zenject;
using _Project.Presentation.Scripts.Controllers;

namespace _Project.Infrastructure.DependencyInjection
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // MainScene specific dependencies like MainSceneAudioOrchestrator for exemple or specific UIManager
            // Controllers
            Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
        }
    }
}