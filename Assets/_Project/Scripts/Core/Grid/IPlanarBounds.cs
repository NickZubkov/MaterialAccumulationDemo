using UnityEngine;

namespace MaterialAccumulation.Core.Grid
{
    /// <summary>Границы плоскости XZ. Зоне нужны только они, а не поле высот целиком.</summary>
    public interface IPlanarBounds
    {
        public Vector2 Clamp(Vector2 position);
    }
}
