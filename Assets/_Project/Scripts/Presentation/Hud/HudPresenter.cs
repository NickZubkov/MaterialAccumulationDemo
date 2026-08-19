using System;
using MaterialAccumulation.Core.Input;
using MaterialAccumulation.Core.Surface;
using MaterialAccumulation.Core.Zone;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Hud
{
    /// <summary>
    /// Связывает состояние системы с пассивной вью.
    /// Опрашивает провайдеры в тике: подписываться не на что — состояние
    /// меняется каждый кадр, и событийная модель дала бы тот же результат дороже.
    /// </summary>
    public sealed class HudPresenter : IInitializable, ITickable, IDisposable
    {
        private const float FrameRateSmoothing = 0.1f;

        /// <summary>Цикл переключателя. Ноль — без ограничения.</summary>
        private static readonly int[] FrameRateLimits = { 0, 20, 60, 200 };

        private readonly IZoneStateProvider _zone;
        private readonly IInputSource _input;
        private readonly ISurfaceResetter _resetter;
        private readonly IViewFactory<IHudView> _viewFactory;

        private IHudView _view;
        private float _smoothedFrameRate;
        private int _limitIndex;
        private int _originalVSyncCount;

        public HudPresenter(
            IZoneStateProvider zone,
            IInputSource input,
            ISurfaceResetter resetter,
            IViewFactory<IHudView> viewFactory)
        {
            _zone = zone;
            _input = input;
            _resetter = resetter;
            _viewFactory = viewFactory;
        }

        public void Initialize()
        {
            _originalVSyncCount = QualitySettings.vSyncCount;

            _view = _viewFactory.Create();
            _view.ResetRequested += OnResetRequested;
            _view.FrameRateLimitRequested += OnFrameRateLimitRequested;

            ApplyFrameRateLimit();
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.ResetRequested -= OnResetRequested;
                _view.FrameRateLimitRequested -= OnFrameRateLimitRequested;
                _view.Dispose();
            }

            // targetFrameRate и vSyncCount глобальны и переживают выход из Play Mode:
            // не вернув их, редактор останется зажатым до перезапуска. Возвращаем их
            // напрямую, а не через ApplyFrameRateLimit: вью к этому моменту может быть
            // уже уничтожен, а исключение отсюда Zenject пробрасывает наружу и обрывает
            // обход — сервисы с меньшим приоритетом не освободили бы Persistent-буферы.
            QualitySettings.vSyncCount = _originalVSyncCount;
            Application.targetFrameRate = -1;
        }

        public void Tick()
        {
            float deltaTime = Time.unscaledDeltaTime;
            if (deltaTime > 0f)
            {
                float instant = 1f / deltaTime;
                _smoothedFrameRate = _smoothedFrameRate <= 0f
                    ? instant
                    : Mathf.Lerp(_smoothedFrameRate, instant, FrameRateSmoothing);
            }

            _view.SetRadius(_zone.Radius);
            _view.SetDepositing(_input.DepositHeld);
            _view.SetFrameRate(_smoothedFrameRate);
        }

        private void OnResetRequested() => _resetter.Reset();

        private void OnFrameRateLimitRequested()
        {
            _limitIndex = (_limitIndex + 1) % FrameRateLimits.Length;
            ApplyFrameRateLimit();
        }

        private void ApplyFrameRateLimit()
        {
            int limit = FrameRateLimits[_limitIndex];

            // При включённом vSync targetFrameRate игнорируется. Режим «без ограничения»
            // возвращает то значение, что стояло в профиле качества, а не жёсткую единицу:
            // в редакторе правка QualitySettings оседает в ассете проекта.
            QualitySettings.vSyncCount = limit > 0 ? 0 : _originalVSyncCount;
            Application.targetFrameRate = limit > 0 ? limit : -1;

            _view.SetFrameRateLimit(limit);
        }
    }
}
