using ColossalFramework.Plugins;
using ForestBrush.Redirection;
using HarmonyLib;
using System;
using UnityEngine;

namespace ForestBrush
{
    public static class Patcher
    {
        private const string harmonyId = "com.tpb.forestbrush";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched)
                return;

            if (!IsModEnabled(593588108UL, "Prop & Tree Anarchy"))
            {
                var harmony = new Harmony(harmonyId);
                harmony.PatchAll(typeof(Patcher).Assembly);
            }
            if (!IsModEnabled(1873351912UL, "Tree Precision"))
            {
                Redirector<PositionDetour>.Deploy();
            }

            patched = true;
        }

        public static void UnpatchAll()
        {
            if (!patched)
                return;

            var harmony = new Harmony(harmonyId);
            if (harmony != null)
            {
                try
                {
                    harmony.UnpatchAll(harmonyId);
                }
                catch (Exception)
                {
                    Debug.LogWarning($"Failed unpatching {harmonyId}, please notify the mod author.");
                }
            }

            Redirector<PositionDetour>.Revert();

            patched = false;
        }

        public static bool IsModEnabled(ulong publishedFileID, string modName)
        {
            foreach (var plugin in PluginManager.instance.GetPluginsInfo())
            {
                if (plugin.publishedFileID.AsUInt64 == publishedFileID
                    || plugin.name == modName)
                {
                    return plugin.isEnabled;
                }
            }
            return false;
        }
    }
}
