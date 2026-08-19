using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Presentation.Surface;
using Zenject;

namespace MaterialAccumulation.Presentation.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<HeightField>().AsSingle();

            Container.BindInterfacesAndSelfTo<MeshSurfaceView>()
                .FromComponentInHierarchy()
                .AsSingle();

            // Порядок кадра задаётся явно: иначе вью отстаёт от состояния на кадр.
            Container.BindExecutionOrder<MeshSurfaceView>(200);
        }
    }
}
