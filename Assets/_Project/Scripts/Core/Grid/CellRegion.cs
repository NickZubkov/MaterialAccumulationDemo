namespace MaterialAccumulation.Core.Grid
{
    /// <summary>
    /// Прямоугольник ячеек. Границы включительные с обеих сторон.
    /// Операции — в CellRegionExtensions.
    /// </summary>
    public readonly struct CellRegion
    {
        public readonly int MinX;
        public readonly int MinZ;
        public readonly int MaxX;
        public readonly int MaxZ;

        public CellRegion(int minX, int minZ, int maxX, int maxZ)
        {
            MinX = minX;
            MinZ = minZ;
            MaxX = maxX;
            MaxZ = maxZ;
        }
    }
}
