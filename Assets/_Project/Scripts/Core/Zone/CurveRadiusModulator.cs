using MaterialAccumulation.Core.Configuration;
using UnityEngine;

namespace MaterialAccumulation.Core.Zone
{
    /// <summary>
    /// Циклическое изменение радиуса по AnimationCurve.
    /// Кривая нормирована на [0,1] по обеим осям и задаёт форму цикла;
    /// масштаб задают амплитуда и частота.
    /// </summary>
    public sealed class CurveRadiusModulator : IRadiusModulator
    {
        private readonly ZoneSettings _settings;

        public CurveRadiusModulator(ZoneSettings settings) => _settings = settings;

        public float Evaluate(float time)
        {
            float phase = Mathf.Repeat(time * _settings.RadiusFrequency, 1f);
            float radius = _settings.BaseRadius + _settings.RadiusAmplitude * _settings.RadiusCurve.Evaluate(phase);
            return Mathf.Max(0f, radius);
        }
    }
}
