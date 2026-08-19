using System;
using UnityEngine;

namespace MaterialAccumulation.Presentation.Zone
{
    /// <summary>Индикатор зоны. Позу считает владелец, вью её только применяет.</summary>
    public interface IZoneMarkerView : IDisposable
    {
        public void SetPose(Vector3 position, float diameter);
    }
}
