using MaterialAccumulation.Core.Input;
using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Core.Zone;
using MaterialAccumulation.Presentation.Surface;
using MaterialAccumulation.Presentation.Zone;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField] private MeshSurfaceView _surfaceViewPrefab;
        [SerializeField] private ZoneMarkerView _zoneMarkerPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SurfaceService>().AsSingle();

            Container.BindInterfacesAndSelfTo<LegacyInputSource>().AsSingle();
            Container.BindInterfacesAndSelfTo<CurveRadiusModulator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZoneMotionService>().AsSingle();

            Container.BindFactory<MeshSurfaceView, SurfaceViewFactory>()
                .FromComponentInNewPrefab(_surfaceViewPrefab);

            Container.BindFactory<ZoneMarkerView, ZoneMarkerViewFactory>()
                .FromComponentInNewPrefab(_zoneMarkerPrefab);

            Container.BindInterfacesAndSelfTo<SurfacePresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZonePresenter>().AsSingle();
        }
    }
}
