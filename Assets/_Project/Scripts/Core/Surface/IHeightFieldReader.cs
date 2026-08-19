using MaterialAccumulation.Core.Grid;
using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    public interface IHeightFieldReader
    {
        public GridGeometry Geometry { get; }
        public NativeArray<float> Heights { get; }
        public bool IsDirty { get; }
        public CellRegion DirtyRegion { get; }

        /// <summary>Вызывается потребителем после того, как грязный регион отрисован.</summary>
        public void ClearDirty();
    }
}
