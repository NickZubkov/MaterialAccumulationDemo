namespace MaterialAccumulation.Core.Surface
{
    /// <summary>Прямоугольник ячеек. Границы включительные с обеих сторон.</summary>
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

        public int Width => MaxX - MinX + 1;
        public int Height => MaxZ - MinZ + 1;
        public int CellCount => Width * Height;
        public bool IsEmpty => MaxX < MinX || MaxZ < MinZ;

        public CellRegion Expand(int margin, in GridGeometry geometry) => new CellRegion(
            geometry.ClampCell(MinX - margin),
            geometry.ClampCell(MinZ - margin),
            geometry.ClampCell(MaxX + margin),
            geometry.ClampCell(MaxZ + margin));

        public CellRegion Union(in CellRegion other) => new CellRegion(
            MinX < other.MinX ? MinX : other.MinX,
            MinZ < other.MinZ ? MinZ : other.MinZ,
            MaxX > other.MaxX ? MaxX : other.MaxX,
            MaxZ > other.MaxZ ? MaxZ : other.MaxZ);
    }
}
