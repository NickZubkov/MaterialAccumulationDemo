using MaterialAccumulation.Core.Configuration;
using MaterialAccumulation.Core.Grid;
using Unity.Collections;
using UnityEngine;

namespace MaterialAccumulation.Core.Deposition
{
    /// <summary>
    /// Однопоточное нанесение свипа. Обходит только AABB свипа,
    /// поэтому стоимость линейна по площади следа и не зависит
    /// ни от размера поверхности, ни от частоты кадров.
    /// Аллокаций не выполняет.
    /// </summary>
    public sealed class ManagedDepositionProcessor : IDepositionProcessor
    {
        private readonly ZoneSettings _settings;

        public ManagedDepositionProcessor(ZoneSettings settings) => _settings = settings;

        public bool Apply(
            NativeArray<float> heights,
            in GridGeometry geometry,
            in DepositionStroke stroke,
            out CellRegion touched)
        {
            touched = SweepMath.ComputeRegion(stroke, geometry);
            if (touched.IsEmpty())
                return false;

            float rate = _settings.AccumulationRate;
            bool anyChanged = false;

            for (int z = touched.MinZ; z <= touched.MaxZ; z++)
            {
                float worldZ = geometry.CellToWorldZ(z);

                for (int x = touched.MinX; x <= touched.MaxX; x++)
                {
                    var cellPosition = new Vector2(geometry.CellToWorldX(x), worldZ);

                    if (!SweepMath.TryEvaluateCell(stroke, cellPosition, out float ceiling, out float dwellTime))
                        continue;

                    int index = geometry.ToIndex(x, z);
                    float current = heights[index];

                    // Купол ограничивает только добавление: то, что выше, не трогаем.
                    if (ceiling <= current)
                        continue;

                    float next = current + rate * dwellTime;
                    heights[index] = next < ceiling ? next : ceiling;
                    anyChanged = true;
                }
            }

            return anyChanged;
        }
    }
}
