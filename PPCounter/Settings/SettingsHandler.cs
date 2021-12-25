using BeatSaberMarkupLanguage.Attributes;

namespace PPCounter.Settings
{
    class SettingsHandler
    {
        [UIValue("decimalPrecision")]
        public int decimalPrecision
        {
            get => PluginSettings.Instance.decimalPrecision;
            set
            {
                PluginSettings.Instance.decimalPrecision = value;
            }
        }

        [UIValue("relativeGain")]
        public bool relativeGain
        {
            get => PluginSettings.Instance.relativeGain;
            set
            {
                PluginSettings.Instance.relativeGain = value;
            }
        }

        [UIValue("relativeGainInline")]
        public bool relativeGainInline
        {
            get => PluginSettings.Instance.relativeGainInline;
            set
            {
                PluginSettings.Instance.relativeGainInline = value;
            }
        }

        [UIValue("relativeGainColor")]
        public bool relativeGainColor
        {
            get => PluginSettings.Instance.relativeGainColor;
            set
            {
                PluginSettings.Instance.relativeGainColor = value;
            }
        }
    }
}
