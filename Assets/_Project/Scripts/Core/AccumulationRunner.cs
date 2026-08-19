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
        /// <summary>Предельная длина подшага как доля радиуса зоны.</summary>
        private const float SubstepRadiusFraction = 0.05f;

        private const int MaxSubsteps = 16;

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

                Deposit(from, _zone.Position, radiusFrom, _zone.Radius, deltaTime);
            }

            _wasDepositing = depositing;
        }

        private void Deposit(Vector2 from, Vector2 to, float radiusFrom, float radiusTo, float deltaTime)
        {
            // Потолок высоты применяется раз на свип и берётся как максимум по всей его
            // длине, поэтому длинный свип обрезает накопление слабее короткого — итог
            // поехал бы вслед за частотой кадров. Дробление на подшаги фиксированной
            // длины делает шаг интегрирования независимым от deltaTime.
            float maxStep = Mathf.Max(radiusFrom, radiusTo) * SubstepRadiusFraction;
            int steps = 1;

            if (maxStep > 0f)
                steps = Mathf.Clamp(Mathf.CeilToInt(Vector2.Distance(from, to) / maxStep), 1, MaxSubsteps);

            float stepTime = deltaTime / steps;
            float inverseSteps = 1f / steps;

            for (int i = 0; i < steps; i++)
            {
                float start = i * inverseSteps;
                float end = (i + 1) * inverseSteps;

                var stroke = new DepositionStroke(
                    Vector2.Lerp(from, to, start),
                    Vector2.Lerp(from, to, end),
                    Mathf.Lerp(radiusFrom, radiusTo, start),
                    Mathf.Lerp(radiusFrom, radiusTo, end),
                    stepTime);

                _surface.Deposit(stroke);
            }
        }
    }
}
