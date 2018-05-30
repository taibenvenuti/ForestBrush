
using ColossalFramework.Plugins;
using System.Linq;

namespace ForestBrush
{
    public class Perk
    {
        public enum Level : ulong
        {
            None,
            Common = 1397372773u,
            Uncommon,
            Rare,
            Epic,
            Mythic,
            Legendary,
            Titan,
            Primordial,
            Count = 8u
        }

        public Perk.Level GetLevel()
        {
            if (PluginManager.instance.GetPluginsInfo().ToList().Find(p => p.publishedFileID.AsUInt64 == (ulong)Level.Common) != null) return Level.Common;
            if (PluginManager.instance.GetPluginsInfo().ToList().Find(p => p.publishedFileID.AsUInt64 == (ulong)Level.Uncommon) != null) return Level.Uncommon;
            return Level.None;
        }
    }
}
