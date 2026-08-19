using UnityEngine;

namespace MaterialAccumulation.Core.Deposition
{
    /// <summary>
    /// Свип зоны за один кадр: отрезок из From в To с линейно меняющимся радиусом.
    /// Штамп в текущей позиции дал бы разрывы следа при высокой скорости
    /// или низком FPS, поэтому наносится именно заметаемый объём.
    /// </summary>
    public readonly struct DepositionStroke
    {
        public readonly Vector2 From;
        public readonly Vector2 To;
        public readonly float RadiusFrom;
        public readonly float RadiusTo;
        public readonly float DeltaTime;

        public DepositionStroke(Vector2 from, Vector2 to, float radiusFrom, float radiusTo, float deltaTime)
        {
            From = from;
            To = to;
            RadiusFrom = radiusFrom;
            RadiusTo = radiusTo;
            DeltaTime = deltaTime;
        }
    }
}
