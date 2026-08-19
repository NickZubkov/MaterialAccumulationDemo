using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Presentation.Surface;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private MeshSurfaceView _surfaceViewPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SurfaceService>().AsSingle();

            Container.BindFactory<MeshSurfaceView, SurfaceViewFactory>()
                .FromComponentInNewPrefab(_surfaceViewPrefab);

            Container.BindInterfacesAndSelfTo<SurfacePresenter>().AsSingle();

            // Порядок кадра задаётся явно: иначе вью отстаёт от состояния на кадр.
            Container.BindExecutionOrder<SurfacePresenter>(200);
        }
    }
}
