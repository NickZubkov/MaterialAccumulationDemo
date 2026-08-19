using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialAccumulation.Presentation.Hud
{
    /// <summary>
    /// Форматирование идёт через TMP_Text.SetText: string.Format аллоцировал бы каждый кадр.
    /// </summary>
    public sealed class HudView : MonoBehaviour, IHudView
    {
        [SerializeField] private TMP_Text _radiusLabel;
        [SerializeField] private TMP_Text _depositingLabel;
        [SerializeField] private TMP_Text _frameRateLabel;
        [SerializeField] private TMP_Text _frameRateLimitLabel;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _frameRateLimitButton;

        public event Action ResetRequested;
        public event Action FrameRateLimitRequested;

        private void Awake()
        {
            _resetButton.onClick.AddListener(RaiseResetRequested);
            _frameRateLimitButton.onClick.AddListener(RaiseFrameRateLimitRequested);
        }

        private void OnDestroy()
        {
            _resetButton.onClick.RemoveListener(RaiseResetRequested);
            _frameRateLimitButton.onClick.RemoveListener(RaiseFrameRateLimitRequested);
        }

        public void Dispose()
        {
            if (this != null)
                Destroy(gameObject);
        }

        public void SetRadius(float radius) => _radiusLabel.SetText("Radius: {0:2} m", radius);

        public void SetDepositing(bool isDepositing) =>
            _depositingLabel.SetText(isDepositing ? "Depositing: YES" : "Depositing: NO");

        public void SetFrameRate(float framesPerSecond) => _frameRateLabel.SetText("FPS: {0:0}", framesPerSecond);

        public void SetFrameRateLimit(int limit)
        {
            if (limit > 0)
                _frameRateLimitLabel.SetText("Limit: {0} FPS", limit);
            else
                _frameRateLimitLabel.SetText("Limit: off");
        }

        private void RaiseResetRequested()
        {
            Deselect();
            ResetRequested?.Invoke();
        }

        private void RaiseFrameRateLimitRequested()
        {
            Deselect();
            FrameRateLimitRequested?.Invoke();
        }

        /// <summary>
        /// Ось Submit в Input Manager привязана в том числе к пробелу, а им наносится
        /// материал: оставшаяся выделенной кнопка срабатывала бы на каждое нанесение.
        /// </summary>
        private void Deselect()
        {
            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
