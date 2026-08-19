using UnityEngine;

namespace MaterialAccumulation.Core.Zone
{
    /// <summary>
    /// Продвижение зоны. Отделено от чтения состояния: читателей много,
    /// а двигать зону вправе только оркестратор кадра.
    /// </summary>
    public interface IZoneMotion
    {
        public void Advance(Vector2 move, float deltaTime);
    }
}
