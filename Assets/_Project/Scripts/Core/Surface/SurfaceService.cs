using System;
using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Deposition;
using MaterialAccumulation.Core.Grid;
using Unity.Collections;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>
    /// Владелец состояния накопленного материала: создаёт буфер высот, отдаёт его
    /// на чтение, наносит свипы и отслеживает грязный регион. Буфер выделяется
    /// один раз и живёт до разрушения контейнера.
    /// </summary>
    public sealed class SurfaceService : ISurfaceReader, ISurfaceResetter, ISurfaceDepositor, IDisposable
    {
        private readonly GridGeometry _geometry;
        private readonly IDepositionProcessor _processor;

        private NativeArray<float> _heights;
        private CellRegion _dirtyRegion;
        private bool _isDirty;

        public GridGeometry Geometry => _geometry;
        public NativeArray<float> Heights => _heights;
        public bool IsDirty => _isDirty;
        public CellRegion DirtyRegion => _dirtyRegion;

        public SurfaceService(SurfaceSettings settings, IDepositionProcessor processor)
        {
            _geometry = new GridGeometry(settings.Resolution, settings.Size);
            _processor = processor;
            _heights = new NativeArray<float>(_geometry.VertexCount(), Allocator.Persistent);
            MarkAllDirty();
        }

        public void Dispose()
        {
            if (_heights.IsCreated)
                _heights.Dispose();
        }

        public void Deposit(in DepositionStroke stroke)
        {
            if (_processor.Apply(_heights, _geometry, stroke, out CellRegion touched))
                MarkDirty(touched);
        }

        public void ClearDirty() => _isDirty = false;

        public void Reset()
        {
            for (int i = 0; i < _heights.Length; i++)
                _heights[i] = 0f;

            MarkAllDirty();
        }

        private void MarkDirty(in CellRegion region)
        {
            if (region.IsEmpty())
                return;

            _dirtyRegion = _isDirty ? _dirtyRegion.Union(region) : region;
            _isDirty = true;
        }

        private void MarkAllDirty()
        {
            int last = _geometry.Resolution - 1;
            _dirtyRegion = new CellRegion(0, 0, last, last);
            _isDirty = true;
        }
    }
}
