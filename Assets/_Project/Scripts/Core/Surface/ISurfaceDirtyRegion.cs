using MaterialAccumulation.Core.Grid;

namespace MaterialAccumulation.Core.Surface
{
    /// <summary>
    /// Регион, изменившийся с прошлой отрисовки. Отделён от чтения поля:
    /// снятие флага — мутация, и в контракте на чтение ей не место.
    /// </summary>
    public interface ISurfaceDirtyRegion
    {
        public bool IsDirty { get; }
        public CellRegion Region { get; }

        /// <summary>Вызывается потребителем после того, как регион отрисован.</summary>
        public void Clear();
    }
}
