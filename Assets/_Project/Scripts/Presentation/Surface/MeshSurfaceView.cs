using Unity.Collections;
using UnityEngine;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>
    /// Держит меш поверхности и заливает в него готовые вершины.
    /// О поле высот и о том, кто им управляет, не знает ничего.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public sealed class MeshSurfaceView : MonoBehaviour, ISurfaceView
    {
        private MeshFilter _meshFilter;
        private Mesh _mesh;

        private void Awake() => _meshFilter = GetComponent<MeshFilter>();

        public void Dispose()
        {
            if (this != null)
                Destroy(gameObject);
        }

        public void SetMesh(Mesh mesh)
        {
            _mesh = mesh;
            _meshFilter.sharedMesh = mesh;
        }

        public void ApplyVertices(NativeArray<SurfaceVertex> vertices, int start, int count) =>
            _mesh.SetVertexBufferData(vertices, start, start, count, 0, SurfaceMeshFlags.Default);
    }
}
