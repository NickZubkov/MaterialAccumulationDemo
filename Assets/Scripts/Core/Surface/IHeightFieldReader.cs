using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    public interface IHeightFieldReader
    {
        GridGeometry Geometry { get; }
        NativeArray<float> Heights { get; }
        bool IsDirty { get; }
        CellRegion DirtyRegion { get; }

        /// <summary>Вызывается потребителем после того, как грязный регион отрисован.</summary>
        void ClearDirty();
    }
}
