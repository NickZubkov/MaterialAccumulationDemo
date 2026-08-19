using Zenject;

namespace MaterialAccumulation.Presentation.Zone
{
    /// <summary>Создание индикатора из префаба. Объявлена отдельно, чтобы вью не зависел от Zenject.</summary>
    public sealed class ZoneMarkerViewFactory : PlaceholderFactory<ZoneMarkerView>, IViewFactory<IZoneMarkerView>
    {
        IZoneMarkerView IViewFactory<IZoneMarkerView>.Create() => Create();
    }
}
