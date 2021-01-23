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
    }
}
