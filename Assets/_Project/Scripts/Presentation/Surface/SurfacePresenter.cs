using System;
using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
using MaterialAccumulation.Core.Surface;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>
    /// Проекция поля высот на меш: создаёт вью, держит вершинный буфер
    /// и в кадре пересчитывает только вершины грязного региона.
    /// </summary>
    public sealed class SurfacePresenter : IInitializable, ITickable, IDisposable
    {
        private readonly ISurfaceReader _surface;
        private readonly SurfaceSettings _settings;
        private readonly SurfaceViewFactory _viewFactory;

        private ISurfaceView _view;
        private Mesh _mesh;
        private NativeArray<SurfaceVertex> _vertices;
        private GridGeometry _geometry;

        public SurfacePresenter(ISurfaceReader surface, SurfaceSettings settings, SurfaceViewFactory viewFactory)
        {
            _surface = surface;
            _settings = settings;
            _viewFactory = viewFactory;
        }

        public void Initialize()
        {
            _geometry = _surface.Geometry;
            _vertices = new NativeArray<SurfaceVertex>(_geometry.VertexCount(), Allocator.Persistent);
            WriteFlatGrid();

            _mesh = SurfaceMeshFactory.Create(_geometry, _settings);

            _view = _viewFactory.Create();
            _view.SetMesh(_mesh);

            UpdateRegion(_surface.DirtyRegion);
            _surface.ClearDirty();
        }

        public void Dispose()
        {
            if (_vertices.IsCreated)
                _vertices.Dispose();

            _view?.Dispose();

            if (_mesh != null)
                UnityEngine.Object.Destroy(_mesh);
        }

        public void Tick()
        {
            if (!_surface.IsDirty)
                return;

            UpdateRegion(_surface.DirtyRegion);
            _surface.ClearDirty();
        }

        private void WriteFlatGrid()
        {
            int resolution = _geometry.Resolution;
            float inverseSpan = 1f / (resolution - 1);

            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int index = _geometry.ToIndex(x, z);
                    var vertex = _vertices[index];
                    vertex.Position = new Vector3(_geometry.CellToWorldX(x), 0f, _geometry.CellToWorldZ(z));
                    vertex.Normal = Vector3.up;
                    vertex.Uv = new Vector2(x * inverseSpan, z * inverseSpan);
                    _vertices[index] = vertex;
                }
            }
        }

        private void UpdateRegion(in CellRegion region)
        {
            // Нормаль краевой ячейки зависит от соседей за границей правки,
            // поэтому пересчитываем на одну ячейку шире.
            CellRegion expanded = region.Expand(1, _geometry);

            int resolution = _geometry.Resolution;
            float cellSize = _geometry.CellSize;
            NativeArray<float> heights = _surface.Heights;

            for (int z = expanded.MinZ; z <= expanded.MaxZ; z++)
            {
                for (int x = expanded.MinX; x <= expanded.MaxX; x++)
                {
                    int index = _geometry.ToIndex(x, z);

                    int left = _geometry.ClampCell(x - 1);
                    int right = _geometry.ClampCell(x + 1);
                    int down = _geometry.ClampCell(z - 1);
                    int up = _geometry.ClampCell(z + 1);

                    float heightLeft = heights[z * resolution + left];
                    float heightRight = heights[z * resolution + right];
                    float heightDown = heights[down * resolution + x];
                    float heightUp = heights[up * resolution + x];

                    // Центральные разности по полю высот. Пролёт берётся фактический:
                    // на кромке ClampCell схлопывает соседа на саму вершину, и деление
                    // на две клетки занизило бы там наклон вдвое.
                    float slopeX = (heightLeft - heightRight) / ((right - left) * cellSize);
                    float slopeZ = (heightDown - heightUp) / ((up - down) * cellSize);

                    var vertex = _vertices[index];
                    vertex.Position.y = heights[index];
                    vertex.Normal = new Vector3(slopeX, 1f, slopeZ).normalized;
                    _vertices[index] = vertex;
                }
            }

            // Вершины лежат построчно, поэтому грязный прямоугольник — непрерывный блок.
            // Заливаем его одним вызовом; лишний захват по краям строк дешевле построчных вызовов.
            int start = expanded.MinZ * resolution + expanded.MinX;
            int end = expanded.MaxZ * resolution + expanded.MaxX;
            _view.ApplyVertices(_vertices, start, end - start + 1);
        }
    }
}
