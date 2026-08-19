using MaterialAccumulation.Core.Deposition;
using MaterialAccumulation.Core.Input;
using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Core.Zone;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Core
{
    /// <summary>
    /// Порядок кадра: продвинуть зону, собрать свип от прежнего положения
    /// к новому, отдать его сервису поверхности. Больше ничего не делает —
    /// вью и HUD читают состояние сами.
    /// </summary>
    public sealed class AccumulationRunner : ITickable
    {
        private readonly IInputSource _input;
        private readonly ZoneMotionService _zone;
        private readonly ISurfaceDepositor _surface;
        private readonly ISurfaceResetter _resetter;

        private bool _wasDepositing;

        public AccumulationRunner(
            IInputSource input,
            ZoneMotionService zone,
            ISurfaceDepositor surface,
            ISurfaceResetter resetter)
        {
            _input = input;
            _zone = zone;
            _surface = surface;
            _resetter = resetter;
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;

            Vector2 previousPosition = _zone.Position;
            float previousRadius = _zone.Radius;

            _zone.Advance(_input.Move, deltaTime);

            if (_input.ResetPressed)
                _resetter.Reset();

            bool depositing = _input.DepositHeld;
            if (depositing && deltaTime > 0f)
            {
                // В первый кадр нажатия свип вырожден: иначе система дорисовала бы след
                // из точки, где зона была при выключенном накоплении.
                Vector2 from = _wasDepositing ? previousPosition : _zone.Position;
                float radiusFrom = _wasDepositing ? previousRadius : _zone.Radius;

                var stroke = new DepositionStroke(from, _zone.Position, radiusFrom, _zone.Radius, deltaTime);
                _surface.Deposit(stroke);
            }

            _wasDepositing = depositing;
        }
    }
}
