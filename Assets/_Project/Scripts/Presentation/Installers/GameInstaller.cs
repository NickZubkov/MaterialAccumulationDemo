using MaterialAccumulation.Core;
using MaterialAccumulation.Core.Deposition;
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
            BindInput();
            BindSurface();
            BindZone();
            BindAccumulation();
            BindTickOrder();
        }

        private void BindInput()
        {
            Container.BindInterfacesAndSelfTo<LegacyInputSource>().AsSingle();
        }

        private void BindSurface()
        {
            Container.BindInterfacesAndSelfTo<ManagedDepositionProcessor>().AsSingle();
            Container.BindInterfacesAndSelfTo<SurfaceService>().AsSingle();

            Container.BindFactory<MeshSurfaceView, SurfaceViewFactory>()
                .FromComponentInNewPrefab(_surfaceViewPrefab);

            Container.BindInterfacesAndSelfTo<SurfacePresenter>().AsSingle();
        }

        private void BindZone()
        {
            Container.BindInterfacesAndSelfTo<CurveRadiusModulator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZoneMotionService>().AsSingle();

            Container.BindFactory<ZoneMarkerView, ZoneMarkerViewFactory>()
                .FromComponentInNewPrefab(_zoneMarkerPrefab);

            Container.BindInterfacesAndSelfTo<ZonePresenter>().AsSingle();
        }

        private void BindAccumulation()
        {
            Container.BindInterfacesAndSelfTo<AccumulationRunner>().AsSingle();
        }

        private void BindTickOrder()
        {
            // Без явных приоритетов все тики получают 0 и идут в порядке биндингов:
            // презентеры отрисовали бы состояние до того, как раннер его изменил.
            Container.BindExecutionOrder<LegacyInputSource>(0);
            Container.BindExecutionOrder<AccumulationRunner>(100);
            Container.BindExecutionOrder<SurfacePresenter>(200);
            Container.BindExecutionOrder<ZonePresenter>(200);
        }
    }
}
