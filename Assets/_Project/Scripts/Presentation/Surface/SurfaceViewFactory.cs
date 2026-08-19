using Zenject;

namespace MaterialAccumulation.Presentation.Surface
{
    /// <summary>Создание вью из префаба. Объявлена отдельно, чтобы вью не зависел от Zenject.</summary>
    public sealed class SurfaceViewFactory : PlaceholderFactory<MeshSurfaceView>
    {
    }
}
