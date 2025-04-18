using Meowdieval.Core.Building;
using Meowdieval.Core.GridSystem;
using Meowdieval.Core.InputHandler;
using Zenject;

namespace Meowdieval.Core.DI
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<EnvironmentGrid>().FromComponentInHierarchy().AsSingle();
			Container.Bind<PlayerInputHandler>().FromComponentInHierarchy().AsSingle();
			Container.Bind<BuildingSystem>().FromComponentInHierarchy().AsSingle();
		}
	}
}
