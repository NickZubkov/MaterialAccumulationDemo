using System.Runtime.InteropServices;
using UnityEngine;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>
    /// Layout вершины в буфере Mesh. Порядок полей обязан совпадать
    /// с VertexAttributeDescriptor в MeshSurfaceView.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SurfaceVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Uv;
    }
}
