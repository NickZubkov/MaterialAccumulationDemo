using MaterialAccumulation.Core.Input;
using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Core.Zone;
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

            Container.BindInterfacesAndSelfTo<LegacyInputSource>().AsSingle();
            Container.Bind<IRadiusModulator>().To<CurveRadiusModulator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZoneMotionService>().AsSingle();

            Container.BindFactory<MeshSurfaceView, SurfaceViewFactory>()
                .FromComponentInNewPrefab(_surfaceViewPrefab);

            Container.BindInterfacesAndSelfTo<SurfacePresenter>().AsSingle();

            // Порядок кадра задаётся явно: иначе вью отстаёт от состояния на кадр.
            Container.BindExecutionOrder<LegacyInputSource>(0);
            Container.BindExecutionOrder<SurfacePresenter>(200);
        }
    }
}
