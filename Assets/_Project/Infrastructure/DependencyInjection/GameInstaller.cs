using Zenject;

namespace _Project.Infrastructure.DependencyInjection
{
    /// <summary>
    /// The GameInstaller is responsible for setting up all dependency bindings for the game scope, which includes scene-specific
    /// bindings and any dependencies that should be instantiated when the game scene is loaded.
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
        }
    }
}