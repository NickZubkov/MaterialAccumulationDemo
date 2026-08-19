using UnityEngine;

namespace MaterialAccumulation.Core.Configuration
{
    [CreateAssetMenu(fileName = "SurfaceSettings", menuName = "MaterialAccumulation/Surface Settings")]
    public sealed class SurfaceSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Сторона квадратной поверхности в метрах.")]
        float _size = 20f;

        [SerializeField, Range(32, 512), Tooltip("Число вершин по стороне сетки.")]
        int _resolution = 256;

        [SerializeField, Tooltip("Верхняя граница Bounds по Y. Должна покрывать максимальный радиус зоны.")]
        float _maxHeight = 3f;

        public float Size => _size;
        public int Resolution => _resolution;
        public float MaxHeight => _maxHeight;
    }
}
