using UnityEngine;

namespace MaterialAccumulation.Core.Configuration
{
    [CreateAssetMenu(fileName = "ZoneSettings", menuName = "MaterialAccumulation/Zone Settings")]
    public sealed class ZoneSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Скорость перемещения зоны, м/с.")]
        float _moveSpeed = 4f;

        [SerializeField, Tooltip("Базовый радиус полусферы, м.")]
        float _baseRadius = 1f;

        [SerializeField, Tooltip("Амплитуда изменения радиуса, м. Умножается на значение кривой.")]
        float _radiusAmplitude = 0.5f;

        [SerializeField, Tooltip("Частота цикла изменения радиуса, Гц.")]
        float _radiusFrequency = 0.5f;

        [SerializeField, Tooltip("Форма цикла радиуса. Нормирована на [0,1] по обеим осям.")]
        AnimationCurve _radiusCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField, Tooltip("Скорость накопления материала, м/с.")]
        float _accumulationRate = 1.5f;

        public float MoveSpeed => _moveSpeed;
        public float BaseRadius => _baseRadius;
        public float RadiusAmplitude => _radiusAmplitude;
        public float RadiusFrequency => _radiusFrequency;
        public AnimationCurve RadiusCurve => _radiusCurve;
        public float AccumulationRate => _accumulationRate;
    }
}
