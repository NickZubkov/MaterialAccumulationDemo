namespace MaterialAccumulation.Presentation
{
    /// <summary>
    /// Создание вью. Презентер зависит от него, а не от конкретной фабрики Zenject:
    /// способ получения объекта (инстанс префаба, пул, заглушка) остаётся подменяемым.
    /// </summary>
    public interface IViewFactory<out TView>
    {
        public TView Create();
    }
}
