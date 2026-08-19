using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>Сборка меша поверхности. Топология строится один раз и дальше не меняется.</summary>
    public static class SurfaceMeshFactory
    {
        public static Mesh Create(in GridGeometry geometry, SurfaceSettings settings)
        {
            int resolution = geometry.Resolution;
            int quadCount = (resolution - 1) * (resolution - 1);
            int indexCount = quadCount * 6;

            var mesh = new Mesh { name = "AccumulationSurface" };

            // 65536 вершин при разрешении 256 — на единицу больше лимита 16-битных индексов.
            mesh.indexFormat = IndexFormat.UInt32;

            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
            };
            mesh.SetVertexBufferParams(geometry.VertexCount(), layout);

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

                mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
                mesh.SetIndexBufferData(indices, 0, 0, indexCount, SurfaceMeshFlags.Default);
            }
            finally
            {
                indices.Dispose();
            }

            mesh.subMeshCount = 1;
            mesh.SetSubMesh(0, new SubMeshDescriptor(0, indexCount), SurfaceMeshFlags.Default);

            // Bounds задаются вручную — обязательное условие при DontRecalculateBounds,
            // иначе поверхность пропадает при frustum culling.
            mesh.bounds = new Bounds(
                new Vector3(0f, settings.MaxHeight * 0.5f, 0f),
                new Vector3(geometry.Size, settings.MaxHeight, geometry.Size));

            return mesh;
        }
    }
}
