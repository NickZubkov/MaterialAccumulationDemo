using MaterialAccumulation.Core.Configuration;
using UnityEngine;
using Zenject;

namespace MaterialAccumulation.Presentation.Installers
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "MaterialAccumulation/Settings Installer")]
    public sealed class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] private SurfaceSettings _surfaceSettings;
        [SerializeField] private ZoneSettings _zoneSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(_surfaceSettings).AsSingle();
            Container.BindInstance(_zoneSettings).AsSingle();
        }
    }
}
