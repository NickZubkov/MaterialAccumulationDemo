using UnityEngine;

namespace MaterialAccumulation.Core.Input
{
    /// <summary>
    /// Снапшот ввода за кадр. Абстракция позволяет подменить источник
    /// (VR-контроллер, воспроизведение записи) без изменений в домене.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>Направление движения в плоскости XZ. Длина не превышает 1.</summary>
        public Vector2 Move { get; }

        public bool DepositHeld { get; }
        public bool ResetPressed { get; }
    }
}
