namespace MaterialAccumulation.Core.Grid
{
    /// <summary>
    /// Параметры регулярной сетки. Сетка центрирована в начале координат:
    /// [-Size/2, +Size/2] по обеим осям. Операции — в GridGeometryExtensions.
    /// </summary>
    public readonly struct GridGeometry
    {
        public readonly int Resolution;
        public readonly float Size;
        public readonly float CellSize;
        public readonly float Half;

        public GridGeometry(int resolution, float size)
        {
            Resolution = resolution;
            Size = size;
            CellSize = size / (resolution - 1);
            Half = size * 0.5f;
        }
    }
}
