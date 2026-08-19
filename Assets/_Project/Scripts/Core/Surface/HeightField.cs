using System;
using MaterialAccumulation.Core.Configuration;
using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>
    /// Состояние накопленного материала. Хранится независимо от отображающего Mesh:
    /// вью читает поле, но поле ничего не знает о вью.
    /// Буфер выделяется один раз и живёт до разрушения контейнера.
    /// </summary>
    public sealed class HeightField : IHeightFieldReader, ISurfaceResetter, IDisposable
    {
        readonly GridGeometry _geometry;

        NativeArray<float> _heights;
        CellRegion _dirtyRegion;
        bool _isDirty;

        public HeightField(SurfaceSettings settings)
        {
            _geometry = new GridGeometry(settings.Resolution, settings.Size);
            _heights = new NativeArray<float>(_geometry.VertexCount, Allocator.Persistent);
            MarkAllDirty();
        }

        public GridGeometry Geometry => _geometry;
        public NativeArray<float> Heights => _heights;
        public bool IsDirty => _isDirty;
        public CellRegion DirtyRegion => _dirtyRegion;

        public float this[int index]
        {
            get => _heights[index];
            set => _heights[index] = value;
        }

        public int ToIndex(int x, int z) => z * _geometry.Resolution + x;

        public void MarkDirty(in CellRegion region)
        {
            if (region.IsEmpty)
                return;

            _dirtyRegion = _isDirty ? _dirtyRegion.Union(region) : region;
            _isDirty = true;
        }

        public void ClearDirty() => _isDirty = false;

        public void Reset()
        {
            for (int i = 0; i < _heights.Length; i++)
                _heights[i] = 0f;

            MarkAllDirty();
        }

        public void Dispose()
        {
            if (_heights.IsCreated)
                _heights.Dispose();
        }

        void MarkAllDirty()
        {
            int last = _geometry.Resolution - 1;
            _dirtyRegion = new CellRegion(0, 0, last, last);
            _isDirty = true;
        }
    }
}
