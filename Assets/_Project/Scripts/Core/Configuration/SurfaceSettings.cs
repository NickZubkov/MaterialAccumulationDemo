using UnityEngine;

namespace MaterialAccumulation.Core.Configuration
{
    [CreateAssetMenu(fileName = "SurfaceSettings", menuName = "MaterialAccumulation/Surface Settings")]
    public sealed class SurfaceSettings : ScriptableObject
    {
        [SerializeField, Tooltip("Сторона квадратной поверхности в метрах.")]
        private float _size = 20f;

        [SerializeField, Range(32, 512), Tooltip("Число вершин по стороне сетки.")]
        private int _resolution = 256;

        public float Size => _size;
        public int Resolution => _resolution;
    }
}
