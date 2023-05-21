using IPA.Config.Stores;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PPCounter.Settings
{
    internal class PluginSettings
    {
        public static Action OnAccSaberEnabled;

        public static PluginSettings Instance { get; set; }
        public virtual bool showIcons { get; set; } = false;
        public virtual bool scoreSaberEnabled { get; set; } = true;
        public virtual bool beatLeaderEnabled { get; set; } = false;
        private bool _accSaberEnabled = false;
        public virtual bool accSaberEnabled
        {
            get => _accSaberEnabled;
            set
            {
                _accSaberEnabled = value;
                if (value)
                {
                    OnAccSaberEnabled?.Invoke();
                }
            }
        }

        // Default Enum ordering
        public virtual long preferredOrder { get; set; } = 123;
        public virtual int numCounters { get; set; } = Enum.GetValues(typeof(PPCounters)).Length;

        public virtual int maxCounters { get; set; } = 5;

        public virtual int decimalPrecision { get; set; } = 2;
        public virtual bool relativeGain { get; set; } = false;
        public virtual bool relativeGainInline { get; set; } = false;
        public virtual bool relativeGainColor { get; set; } = false;
    }

    internal enum PPCounters
    {
        ScoreSaber = 1,
        BeatLeader = 2,
        AccSaber = 3
    }
}