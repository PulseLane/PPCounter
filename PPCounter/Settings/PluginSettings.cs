using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PPCounter.Settings
{
    internal class PluginSettings
    {
        public static PluginSettings Instance { get; set; }

        public virtual bool ignoreNoFail { get; set; } = true;
        public virtual int decimalPrecision { get; set; } = 2;
    }
}