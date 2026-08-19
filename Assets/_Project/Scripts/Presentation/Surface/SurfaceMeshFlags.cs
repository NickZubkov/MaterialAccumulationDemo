using UnityEngine.Rendering;

namespace MaterialAccumulation.Presentation.Surface
{
    internal static class SurfaceMeshFlags
    {
        /// <summary>
        /// Один набор флагов на сборку меша и на заливку вершин: разойдутся —
        /// Unity начнёт пересчитывать Bounds по буферу, который для этого не полон.
        /// </summary>
        public const MeshUpdateFlags Default =
            MeshUpdateFlags.DontRecalculateBounds |
            MeshUpdateFlags.DontValidateIndices |
            MeshUpdateFlags.DontNotifyMeshUsers;
    }
}
