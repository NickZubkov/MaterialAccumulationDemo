using UnityEngine;

namespace MaterialAccumulation.Presentation.Zone
{
    /// <summary>
    /// Визуальный индикатор зоны воздействия. Один постоянный объект:
    /// порции материала отдельными объектами не создаются.
    /// </summary>
    public sealed class ZoneMarkerView : MonoBehaviour, IZoneMarkerView
    {
        public void Dispose()
        {
            if (this != null)
                Destroy(gameObject);
        }

        public void SetPose(Vector3 position, float diameter)
        {
            transform.localPosition = position;
            transform.localScale = new Vector3(diameter, diameter, diameter);
        }
    }
}
