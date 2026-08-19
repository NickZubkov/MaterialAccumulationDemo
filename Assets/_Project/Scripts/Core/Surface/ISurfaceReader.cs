using MaterialAccumulation.Core.Grid;
using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>Состояние поверхности на чтение. Запись идёт только через владельца.</summary>
    public interface ISurfaceReader
    {
        public GridGeometry Geometry { get; }
        public NativeArray<float>.ReadOnly Heights { get; }
        public bool IsDirty { get; }
        public CellRegion DirtyRegion { get; }

        /// <summary>Вызывается потребителем после того, как грязный регион отрисован.</summary>
        public void ClearDirty();
    }
}
