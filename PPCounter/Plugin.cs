using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;

namespace PPCounter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "PP Counter";

        [Init]
        public Plugin(IPALogger logger, Config config)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
            Settings.PluginSettings.Instance = config.Generated<Settings.PluginSettings>();
        }

        [OnEnable]
        public void OnEnable()
        {
            RegisterInstallers();
        }

        [OnDisable]
        public void OnDisable()
        {
            UnregisterInstallers();
        }

        private void RegisterInstallers()
        {
            SiraUtil.Zenject.Installer.RegisterAppInstaller<Installers.DataInstaller>();
            SiraUtil.Zenject.Installer.RegisterGameplayCoreInstaller<Installers.UtilsInstaller>();
        }
        private void UnregisterInstallers()
        {
            SiraUtil.Zenject.Installer.UnregisterAppInstaller<Installers.DataInstaller>();
            SiraUtil.Zenject.Installer.UnregisterGameplayCoreInstaller<Installers.UtilsInstaller>();
        }
    }
}
