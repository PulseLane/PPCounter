using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace PPCounter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "PP Counter";

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenject)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
            zenject.Install<Installers.DataInstaller>(Location.App);
            zenject.Install<Installers.UtilsInstaller>(Location.App);
            Settings.PluginSettings.Instance = config.Generated<Settings.PluginSettings>();
        }
    }
}
