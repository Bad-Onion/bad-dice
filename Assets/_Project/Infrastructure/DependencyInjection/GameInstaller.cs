using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // MainScene specific dependencies like MainSceneAudioOrchestrator for exemple or specific UIManager
        }
    }
}