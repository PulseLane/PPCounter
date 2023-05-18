using PPCounter.Counters;
using PPCounter.Settings;
using Zenject;

namespace PPCounter.Installers
{
    class PPCounterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            if (PluginSettings.Instance.scoreSaberEnabled)
            {
                Container.BindInterfacesAndSelfTo<ScoreSaberCounter>().AsSingle();
            }
            if (PluginSettings.Instance.beatLeaderEnabled)
            {
                Container.BindInterfacesAndSelfTo<BeatLeaderCounter>().AsSingle();
            }
            if (PluginSettings.Instance.accSaberEnabled)
            {
                Container.BindInterfacesAndSelfTo<AccSaberCounter>().AsSingle();
            }
        }
    }
}
