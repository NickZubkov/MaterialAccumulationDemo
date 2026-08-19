using UnityEngine;

namespace MaterialAccumulation.Core.Grid
{
    /// <summary>
    /// Операции над параметрами сетки. Вынесены из структуры, чтобы та осталась
    /// чистыми данными; Burst компилирует их наравне с обычной статикой.
    /// </summary>
    public static class GridGeometryExtensions
    {
        public static int VertexCount(this in GridGeometry geometry) =>
            geometry.Resolution * geometry.Resolution;

        public static int ToIndex(this in GridGeometry geometry, int x, int z) =>
            z * geometry.Resolution + x;

        public static float CellToWorldX(this in GridGeometry geometry, int x) =>
            -geometry.Half + x * geometry.CellSize;

        public static float CellToWorldZ(this in GridGeometry geometry, int z) =>
            -geometry.Half + z * geometry.CellSize;

        public static int FloorCell(this in GridGeometry geometry, float world) =>
            Mathf.FloorToInt((world + geometry.Half) / geometry.CellSize);

        public static int CeilCell(this in GridGeometry geometry, float world) =>
            Mathf.CeilToInt((world + geometry.Half) / geometry.CellSize);

        public static int ClampCell(this in GridGeometry geometry, int cell) =>
            cell < 0 ? 0 : (cell >= geometry.Resolution ? geometry.Resolution - 1 : cell);

        public static Vector2 ClampToBounds(this in GridGeometry geometry, Vector2 position) => new Vector2(
            Mathf.Clamp(position.x, -geometry.Half, geometry.Half),
            Mathf.Clamp(position.y, -geometry.Half, geometry.Half));
    }
}
