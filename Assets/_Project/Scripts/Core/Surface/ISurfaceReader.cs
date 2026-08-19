using MaterialAccumulation.Core.Grid;
using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>Состояние поверхности на чтение.</summary>
    public interface ISurfaceReader
    {
        public GridGeometry Geometry { get; }
        public NativeArray<float>.ReadOnly Heights { get; }
    }
}
