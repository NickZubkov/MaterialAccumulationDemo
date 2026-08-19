using System;
using Unity.Collections;
using UnityEngine;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>Отображение поверхности. Обновлением управляет владелец, вью только применяет.</summary>
    public interface ISurfaceView : IDisposable
    {
        public void SetMesh(Mesh mesh);
        public void ApplyVertices(NativeArray<SurfaceVertex> vertices, int start, int count);
    }
}
