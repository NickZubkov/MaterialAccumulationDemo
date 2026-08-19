using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Core.Input
{
    /// <summary>Legacy Input Manager. Снимает состояние один раз в кадр, до всех потребителей.</summary>
    public sealed class LegacyInputSource : IInputSource, ITickable
    {
        public Vector2 Move { get; private set; }
        public bool DepositHeld { get; private set; }
        public bool ResetPressed { get; private set; }

        public void Tick()
        {
            var move = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical"));

            // Без нормализации диагональ была бы в 1.41 раза быстрее прямой.
            if (move.sqrMagnitude > 1f)
                move.Normalize();

            Move = move;
            DepositHeld = UnityEngine.Input.GetKey(KeyCode.Space);
            ResetPressed = UnityEngine.Input.GetKeyDown(KeyCode.R);
        }
    }
}
