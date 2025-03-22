using Meowdieval.Core.Ui;
using Meowdieval.Core.Utils;
using Zenject;

namespace Meowdieval.Core.DI
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Bind WindowEnvironmentDragger as a single instance
            Container.Bind<WindowEnvironmentDragger>().FromComponentInHierarchy().AsSingle();

            // Bind WindowEnvironmentScaler as a single instance
            Container.Bind<WindowEnvironmentScaler>().FromComponentInHierarchy().AsSingle();

            // Bind UiCanvasManager as a single instance
            Container.Bind<UiCanvasManager>().FromComponentInHierarchy().AsSingle();

            // Bind TransparentWindow as a single instance
            Container.Bind<TransparentWindow>().FromComponentInHierarchy().AsSingle();
        }
    }
}
