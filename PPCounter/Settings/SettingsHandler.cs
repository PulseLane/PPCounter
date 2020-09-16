using BeatSaberMarkupLanguage.Attributes;

namespace PPCounter.Settings
{
    class SettingsHandler
    {
        [UIValue("ignoreNoFail")]
        public bool ignoreNoFail
        {
            get => PluginSettings.Instance.ignoreNoFail;
            set
            {
                PluginSettings.Instance.ignoreNoFail = value;
            }
        }

        [UIValue("decimalPrecision")]
        public int decimalPrecision
        {
            get => PluginSettings.Instance.decimalPrecision;
            set
            {
                PluginSettings.Instance.decimalPrecision = value;
            }
        }
    }
}
