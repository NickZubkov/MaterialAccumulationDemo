using System;
using MaterialAccumulation.Core.Zone;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Zone
{
    /// <summary>Держит индикатор зоны в актуальной позе. Состояние читает, но не меняет.</summary>
    public sealed class ZonePresenter : IInitializable, ITickable, IDisposable
    {
        private readonly IZoneStateProvider _zone;
        private readonly IViewFactory<IZoneMarkerView> _viewFactory;

        private IZoneMarkerView _view;

        public ZonePresenter(IZoneStateProvider zone, IViewFactory<IZoneMarkerView> viewFactory)
        {
            _zone = zone;
            _viewFactory = viewFactory;
        }

        public void Initialize() => _view = _viewFactory.Create();

        public void Dispose() => _view?.Dispose();

        public void Tick()
        {
            Vector2 position = _zone.Position;

            // Примитив Sphere имеет диаметр 1 при единичном масштабе.
            _view.SetPose(new Vector3(position.x, 0f, position.y), _zone.Radius * 2f);
        }
    }
}
