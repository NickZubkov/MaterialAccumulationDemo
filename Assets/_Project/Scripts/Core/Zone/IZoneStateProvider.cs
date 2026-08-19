using UnityEngine;

namespace MaterialAccumulation.Core.Zone
{
    /// <summary>Текущее состояние зоны для вью и HUD. Только чтение.</summary>
    public interface IZoneStateProvider
    {
        /// <summary>Позиция центра зоны в плоскости XZ.</summary>
        public Vector2 Position { get; }

        public float Radius { get; }
    }
}
