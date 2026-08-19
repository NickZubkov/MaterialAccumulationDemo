using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
using MaterialAccumulation.Core.Surface;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>
    /// Проекция поля высот на Mesh. Топология строится один раз;
    /// в кадре обновляются только вершины грязного региона.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public sealed class MeshSurfaceView : MonoBehaviour, IInitializable, ITickable, System.IDisposable
    {
        private const MeshUpdateFlags UpdateFlags =
            MeshUpdateFlags.DontRecalculateBounds |
            MeshUpdateFlags.DontValidateIndices |
            MeshUpdateFlags.DontNotifyMeshUsers;

        private IHeightFieldReader _field;
        private SurfaceSettings _settings;

        private Mesh _mesh;
        private NativeArray<SurfaceVertex> _vertices;
        private GridGeometry _geometry;

        [Inject]
        private void Construct(IHeightFieldReader field, SurfaceSettings settings)
        {
            _field = field;
            _settings = settings;
        }

        public void Initialize()
        {
            _geometry = _field.Geometry;
            BuildMesh();
            UpdateRegion(_field.DirtyRegion);
            _field.ClearDirty();
        }

        public void Dispose()
        {
            if (_vertices.IsCreated)
                _vertices.Dispose();

            if (_mesh != null)
                Destroy(_mesh);
        }

        public void Tick()
        {
            if (!_field.IsDirty)
                return;

            UpdateRegion(_field.DirtyRegion);
            _field.ClearDirty();
        }

        private void BuildMesh()
        {
            int resolution = _geometry.Resolution;
            int vertexCount = _geometry.VertexCount();
            int quadCount = (resolution - 1) * (resolution - 1);
            int indexCount = quadCount * 6;

            _vertices = new NativeArray<SurfaceVertex>(vertexCount, Allocator.Persistent);

            _mesh = new Mesh { name = "AccumulationSurface" };

            // 65536 вершин при разрешении 256 — на единицу больше лимита 16-битных индексов.
            _mesh.indexFormat = IndexFormat.UInt32;

            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            };
            _mesh.SetVertexBufferParams(vertexCount, layout);

            // Сеттер индексатора NativeArray помечен [WriteAccessRequired], а переменная
            // using — readonly: запись по индексу внутри using не компилируется (CS1654).
            var indices = new NativeArray<int>(indexCount, Allocator.Temp);
            try
            {
                int cursor = 0;
                for (int z = 0; z < resolution - 1; z++)
                {
                    for (int x = 0; x < resolution - 1; x++)
                    {
                        int origin = z * resolution + x;
                        int above = origin + resolution;

                        indices[cursor++] = origin;
                        indices[cursor++] = above;
                        indices[cursor++] = origin + 1;

                        indices[cursor++] = origin + 1;
                        indices[cursor++] = above;
                        indices[cursor++] = above + 1;
                    }
                }

                _mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
                _mesh.SetIndexBufferData(indices, 0, 0, indexCount, UpdateFlags);
            }
            finally
            {
                indices.Dispose();
            }

            _mesh.subMeshCount = 1;
            _mesh.SetSubMesh(0, new SubMeshDescriptor(0, indexCount), UpdateFlags);

            // Bounds задаются вручную — обязательное условие при DontRecalculateBounds,
            // иначе поверхность пропадает при frustum culling.
            _mesh.bounds = new Bounds(
                new Vector3(0f, _settings.MaxHeight * 0.5f, 0f),
                new Vector3(_geometry.Size, _settings.MaxHeight, _geometry.Size));

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            float inverseSpan = 1f / (resolution - 1);
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int index = z * resolution + x;
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
            float doubleCell = 2f * _geometry.CellSize;
            NativeArray<float> heights = _field.Heights;

            for (int z = expanded.MinZ; z <= expanded.MaxZ; z++)
            {
                for (int x = expanded.MinX; x <= expanded.MaxX; x++)
                {
                    int index = z * resolution + x;

                    int left = _geometry.ClampCell(x - 1);
                    int right = _geometry.ClampCell(x + 1);
                    int down = _geometry.ClampCell(z - 1);
                    int up = _geometry.ClampCell(z + 1);

                    float heightLeft = heights[z * resolution + left];
                    float heightRight = heights[z * resolution + right];
                    float heightDown = heights[down * resolution + x];
                    float heightUp = heights[up * resolution + x];

                    var vertex = _vertices[index];
                    vertex.Position.y = heights[index];
                    // Центральные разности по полю высот.
                    vertex.Normal = new Vector3(
                        heightLeft - heightRight,
                        doubleCell,
                        heightDown - heightUp).normalized;
                    _vertices[index] = vertex;
                }
            }

            // Вершины лежат построчно, поэтому грязный прямоугольник — непрерывный блок.
            // Заливаем его одним вызовом; лишний захват по краям строк дешевле построчных вызовов.
            int start = expanded.MinZ * resolution + expanded.MinX;
            int end = expanded.MaxZ * resolution + expanded.MaxX;
            _mesh.SetVertexBufferData(_vertices, start, start, end - start + 1, 0, UpdateFlags);
        }
    }
}
