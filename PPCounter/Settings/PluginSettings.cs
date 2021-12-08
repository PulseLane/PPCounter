using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PPCounter.Settings
{
    internal class PluginSettings
    {
        public static PluginSettings Instance { get; set; }
        public virtual int decimalPrecision { get; set; } = 2;
        public virtual bool newCurve { get; set; } = false;
        public virtual bool relativeGain { get; set; } = false;
        public virtual bool relativeGainInline { get; set; } = false;
        public virtual bool relativeGainColor { get; set; } = false;
    }
}