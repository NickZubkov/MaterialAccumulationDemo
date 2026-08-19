using Zenject;

namespace MaterialAccumulation.Presentation.Hud
{
    /// <summary>Создание HUD из префаба. Объявлена отдельно, чтобы вью не зависел от Zenject.</summary>
    public sealed class HudViewFactory : PlaceholderFactory<HudView>
    {
    }
}
