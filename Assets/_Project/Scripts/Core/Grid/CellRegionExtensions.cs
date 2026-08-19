namespace MaterialAccumulation.Core.Grid
{
    /// <summary>Операции над прямоугольником ячеек.</summary>
    public static class CellRegionExtensions
    {
        public static int Width(this in CellRegion region) => region.MaxX - region.MinX + 1;

        public static int Height(this in CellRegion region) => region.MaxZ - region.MinZ + 1;

        public static int CellCount(this in CellRegion region) => region.Width() * region.Height();

        public static bool IsEmpty(this in CellRegion region) =>
            region.MaxX < region.MinX || region.MaxZ < region.MinZ;

        public static CellRegion Expand(this in CellRegion region, int margin, in GridGeometry geometry) =>
            new CellRegion(
                geometry.ClampCell(region.MinX - margin),
                geometry.ClampCell(region.MinZ - margin),
                geometry.ClampCell(region.MaxX + margin),
                geometry.ClampCell(region.MaxZ + margin));

        public static CellRegion Union(this in CellRegion region, in CellRegion other) =>
            new CellRegion(
                region.MinX < other.MinX ? region.MinX : other.MinX,
                region.MinZ < other.MinZ ? region.MinZ : other.MinZ,
                region.MaxX > other.MaxX ? region.MaxX : other.MaxX,
                region.MaxZ > other.MaxZ ? region.MaxZ : other.MaxZ);
    }
}
