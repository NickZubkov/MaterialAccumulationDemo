using MaterialAccumulation.Core.Grid;
using UnityEngine;

namespace MaterialAccumulation.Core.Deposition
{
    /// <summary>
    /// Геометрия свипа. Вынесена отдельно, чтобы managed- и Burst-реализации
    /// считали по одним и тем же формулам.
    /// </summary>
    public static class SweepMath
    {
        public const float Epsilon = 1e-6f;

        /// <summary>Прямоугольник ячеек, покрывающий AABB свипа, обрезанный границами сетки.</summary>
        public static CellRegion ComputeRegion(in DepositionStroke stroke, in GridGeometry geometry)
        {
            float maxRadius = Mathf.Max(stroke.RadiusFrom, stroke.RadiusTo);

            float minX = Mathf.Min(stroke.From.x, stroke.To.x) - maxRadius;
            float maxX = Mathf.Max(stroke.From.x, stroke.To.x) + maxRadius;
            float minZ = Mathf.Min(stroke.From.y, stroke.To.y) - maxRadius;
            float maxZ = Mathf.Max(stroke.From.y, stroke.To.y) + maxRadius;

            return new CellRegion(
                geometry.ClampCell(geometry.FloorCell(minX)),
                geometry.ClampCell(geometry.FloorCell(minZ)),
                geometry.ClampCell(geometry.CeilCell(maxX)),
                geometry.ClampCell(geometry.CeilCell(maxZ)));
        }

        /// <summary>
        /// Для точки cellPosition считает потолок высоты и время пребывания под зоной.
        /// Возвращает false, если точка свипом не задета.
        ///
        /// Обозначения: d = p - From, ab = To - From, dr = RadiusTo - RadiusFrom.
        /// f(t) = r(t)^2 - |d - t*ab|^2 = -A*t^2 + 2*B*t + C, где
        ///   A = |ab|^2 - dr^2, B = dot(d, ab) + RadiusFrom*dr, C = RadiusFrom^2 - |d|^2.
        /// f(t) > 0 означает, что в момент t точка находится под куполом.
        /// </summary>
        public static bool TryEvaluateCell(
            in DepositionStroke stroke,
            Vector2 cellPosition,
            out float ceiling,
            out float dwellTime)
        {
            ceiling = 0f;
            dwellTime = 0f;

            Vector2 ab = stroke.To - stroke.From;
            Vector2 d = cellPosition - stroke.From;

            float abSq = Vector2.Dot(ab, ab);
            float dSq = Vector2.Dot(d, d);
            float dab = Vector2.Dot(d, ab);

            // --- Время пребывания ---
            // Считается по капсуле со средним радиусом: тогда старший коэффициент равен
            // |ab|^2 >= 0, парабола гарантированно ветвями вниз и вырожденных ветвлений нет.
            // Разница радиусов за кадр — сантиметры, её вклад в длительность пренебрежим.
            float meanRadius = 0.5f * (stroke.RadiusFrom + stroke.RadiusTo);
            float meanRadiusSq = meanRadius * meanRadius;

            if (abSq > Epsilon)
            {
                float discriminant = dab * dab + abSq * (meanRadiusSq - dSq);
                if (discriminant <= 0f)
                    return false;

                float root = Mathf.Sqrt(discriminant);
                float inverseAbSq = 1f / abSq;
                float enter = Mathf.Max(0f, (dab - root) * inverseAbSq);
                float exit = Mathf.Min(1f, (dab + root) * inverseAbSq);

                if (exit <= enter)
                    return false;

                dwellTime = (exit - enter) * stroke.DeltaTime;
            }
            else
            {
                if (dSq >= meanRadiusSq)
                    return false;

                dwellTime = stroke.DeltaTime;
            }

            // --- Потолок высоты ---
            // Считается точно, с переменным радиусом: потолок определяет форму следа.
            float dr = stroke.RadiusTo - stroke.RadiusFrom;
            float a = abSq - dr * dr;
            float b = dab + stroke.RadiusFrom * dr;
            float c = stroke.RadiusFrom * stroke.RadiusFrom - dSq;

            float best;
            if (a > Epsilon)
            {
                // Вершина параболы, зажатая в отрезок [0,1].
                float t = Mathf.Clamp01(b / a);
                best = -a * t * t + 2f * b * t + c;
            }
            else
            {
                // Радиус растёт быстрее, чем движется зона: максимум на одном из концов.
                float atStart = c;
                float atEnd = -a + 2f * b + c;
                best = Mathf.Max(atStart, atEnd);
            }

            if (best <= 0f)
                return false;

            ceiling = Mathf.Sqrt(best);
            return true;
        }
    }
}
