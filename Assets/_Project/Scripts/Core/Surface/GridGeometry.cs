using UnityEngine;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>
    /// Отображение мировых координат XZ на индексы регулярной сетки.
    /// Сетка центрирована в начале координат: [-Size/2, +Size/2] по обеим осям.
    /// </summary>
    public readonly struct GridGeometry
    {
        public readonly int Resolution;
        public readonly float Size;
        public readonly float CellSize;

        readonly float _half;

        public GridGeometry(int resolution, float size)
        {
            Resolution = resolution;
            Size = size;
            CellSize = size / (resolution - 1);
            _half = size * 0.5f;
        }

        public int VertexCount => Resolution * Resolution;

        public float CellToWorldX(int x) => -_half + x * CellSize;
        public float CellToWorldZ(int z) => -_half + z * CellSize;

        public int FloorCell(float world) => Mathf.FloorToInt((world + _half) / CellSize);
        public int CeilCell(float world) => Mathf.CeilToInt((world + _half) / CellSize);

        public int ClampCell(int cell) =>
            cell < 0 ? 0 : (cell >= Resolution ? Resolution - 1 : cell);

        public Vector2 ClampToBounds(Vector2 position) => new Vector2(
            Mathf.Clamp(position.x, -_half, _half),
            Mathf.Clamp(position.y, -_half, _half));
    }
}
