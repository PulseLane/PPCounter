using IPA;
using IPA.Config;
using IPA.Config.Stores;
using PPCounter.Settings;
using PPCounter.Utilities;
using SiraUtil.Zenject;
using System;
using System.Collections.Generic;
using IPALogger = IPA.Logging.Logger;

namespace PPCounter
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "PP Counter";

        //private readonly Harmony _harmony;
        //private const string _harmonyID = "dev.PulseLane.BeatSaber.PPCounter";

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenject)
        {
            instance = this;
            Logger.log = logger;
            zenject.Install<Installers.DataInstaller>(Location.App);
            zenject.Install<Installers.CalculatorsInstaller>(Location.App);
            zenject.Install<Installers.PPCounterInstaller>(Location.StandardPlayer, Location.MultiPlayer);
            PluginSettings.Instance = config.Generated<PluginSettings>();


            RenewSettings();
            //_harmony = new Harmony(_harmonyID);

            //if (IsBeatLeaderInstalled())
            //{
            //    PatchBeatLeader();
            //}
        }

        private void RenewSettings()
        {
            var enumCount = Enum.GetValues(typeof(PPCounters)).Length;
            if (PluginSettings.Instance.numCounters < enumCount)
            {
                List<PPCounters> order = SettingsUtils.GetCounterOrder(PluginSettings.Instance.preferredOrder, PluginSettings.Instance.numCounters);
                foreach (PPCounters counter in Enum.GetValues(typeof(PPCounters)))
                {
                    if (!order.Contains(counter))
                    {
                        order.Add(counter);
                    }
                }

                PluginSettings.Instance.preferredOrder = SettingsUtils.GetPreferredOrderNumber(order);
                PluginSettings.Instance.numCounters = enumCount;
            }
        }

        //private void PatchBeatLeader()
        //{
        //    PluginMetadata metadata = PluginManager.GetPluginFromId("BeatLeader");

        //    // BeatLeader.DataManager.LeaderbaordsCache.NotifyCacheWasChanged()
        //    var originalNotifyCacheWasChanged = metadata.Assembly.GetType("BeatLeader.DataManager.LeaderbaordsCache").GetMethod("NotifyCacheWasChanged", (BindingFlags)(-1));
        //    HarmonyMethod harmonyNotifyCacheWasChanged = new HarmonyMethod(typeof(LeaderboardsCacheNotifyCacheWasChangedPatch).GetMethod("Postfix", (BindingFlags)(-1)));
        //    _harmony.Patch(originalNotifyCacheWasChanged, harmonyNotifyCacheWasChanged);
        //}

        //private bool IsBeatLeaderInstalled()
        //{
        //    try
        //    {
        //        var metadatas = PluginManager.EnabledPlugins.Where(x => x.Id == "BeatLeader");
        //        return metadatas.Count() > 0;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.log.Debug($"Error checking for BeatLeader install: {e.Message}");
        //        return false;
        //    }
        //}
    }
}
