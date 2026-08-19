using System;
using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
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
        private readonly GridGeometry _geometry;

        private NativeArray<float> _heights;
        private CellRegion _dirtyRegion;
        private bool _isDirty;

        public GridGeometry Geometry => _geometry;
        public NativeArray<float> Heights => _heights;
        public bool IsDirty => _isDirty;
        public CellRegion DirtyRegion => _dirtyRegion;

        public float this[int index]
        {
            get => _heights[index];
            set => _heights[index] = value;
        }

        public HeightField(SurfaceSettings settings)
        {
            _geometry = new GridGeometry(settings.Resolution, settings.Size);
            _heights = new NativeArray<float>(_geometry.VertexCount(), Allocator.Persistent);
            MarkAllDirty();
        }

        public void Dispose()
        {
            if (_heights.IsCreated)
                _heights.Dispose();
        }

        public void MarkDirty(in CellRegion region)
        {
            if (region.IsEmpty())
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

        private void MarkAllDirty()
        {
            int last = _geometry.Resolution - 1;
            _dirtyRegion = new CellRegion(0, 0, last, last);
            _isDirty = true;
        }
    }
}
