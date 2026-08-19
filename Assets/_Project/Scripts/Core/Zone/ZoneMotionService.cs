using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
using MaterialAccumulation.Core.Surface;
using UnityEngine;

namespace MaterialAccumulation.Core.Zone
{
    /// <summary>
    /// Движение зоны и её радиус. Тик не реализует намеренно:
    /// продвижением управляет AccumulationRunner, которому нужно
    /// предыдущее состояние зоны для построения свипа.
    /// </summary>
    public sealed class ZoneMotionService : IZoneStateProvider
    {
        private readonly ZoneSettings _settings;
        private readonly IRadiusModulator _modulator;
        private readonly GridGeometry _geometry;

        private float _time;

        public Vector2 Position { get; private set; }
        public float Radius { get; private set; }

        public ZoneMotionService(ZoneSettings settings, IRadiusModulator modulator, ISurfaceReader field)
        {
            _settings = settings;
            _modulator = modulator;
            _geometry = field.Geometry;
            Radius = _modulator.Evaluate(0f);
        }

        public void Advance(Vector2 move, float deltaTime)
        {
            _time += deltaTime;

            // Клампим позицию границами поля, чтобы AABB свипа не выходил за массив.
            Position = _geometry.ClampToBounds(Position + move * (_settings.MoveSpeed * deltaTime));
            Radius = _modulator.Evaluate(_time);
        }
    }
}
