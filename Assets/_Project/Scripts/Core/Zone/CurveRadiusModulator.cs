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

        public CurveRadiusModulator(ZoneSettings settings)
        {
            _settings = settings;
        }

        public float Evaluate(float time)
        {
            float phase = Mathf.Repeat(time * _settings.RadiusFrequency, 1f);
            float radius = _settings.BaseRadius + _settings.RadiusAmplitude * _settings.RadiusCurve.Evaluate(phase);

            // Кривая нормирована на [0,1] по договорённости, но нарисовать можно любую.
            // Верхняя граница здесь делает договорённость гарантией: из неё выводятся
            // Bounds меша, и радиус сверх неё выронил бы поверхность из frustum culling.
            return Mathf.Clamp(radius, 0f, _settings.BaseRadius + _settings.RadiusAmplitude);
        }
    }
}
