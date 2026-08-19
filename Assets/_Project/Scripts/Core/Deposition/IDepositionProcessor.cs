using MaterialAccumulation.Core.Grid;
using Unity.Collections;

namespace MaterialAccumulation.Core.Deposition
{
    public interface IDepositionProcessor
    {
        /// <summary>
        /// Наносит свип на буфер высот. Возвращает false, если ни одна ячейка не изменилась;
        /// touched — прямоугольник, который нужно перезалить во вью.
        /// </summary>
        public bool Apply(
            NativeArray<float> heights,
            in GridGeometry geometry,
            in DepositionStroke stroke,
            out CellRegion touched);
    }
}
