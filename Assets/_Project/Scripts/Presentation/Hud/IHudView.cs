using System;

namespace MaterialAccumulation.Presentation.Hud
{
    /// <summary>Пассивная вью: умеет только показывать значения и сообщать о нажатиях.</summary>
    public interface IHudView : IDisposable
    {
        public event Action ResetRequested;
        public event Action FrameRateLimitRequested;

        public void SetRadius(float radius);
        public void SetDepositing(bool isDepositing);
        public void SetFrameRate(float framesPerSecond);
        public void SetFrameRateLimit(int limit);
    }
}
