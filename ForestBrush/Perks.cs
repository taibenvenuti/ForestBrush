
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ForestBrush
{
    public interface IForestBrushPerk
    {
        void Initialize(ForestBrushSettings settings, UIPanel forestBrushPanel);
    }

    public class ForestBrushPerks
    {
        private static IForestBrushPerk[] perks = { };

        public static void Initialize()
        {
            var pluginsList = new List<IForestBrushPerk>();
            var enabledMods = PluginManager.instance.GetPluginsInfo().Where(p => p.isEnabled);
            foreach (var mod in enabledMods)
            {
                var assemblies = GetPrivate<List<Assembly>>(mod, "m_Assemblies");
                if (assemblies == null)
                {
                    continue;
                }
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();
                        pluginsList.AddRange(types
                            .Where(type => typeof(IForestBrushPerk).IsAssignableFrom(type))
                            .Select(type => (IForestBrushPerk)Activator.CreateInstance(type))
                            );
                    }
                    catch
                    {
                        
                    }
                }
            }
            perks = pluginsList.ToArray();
        }

        public static void Apply()
        {
            if (perks.Length < 1) return;
            foreach (var plugin in perks)
            {
                plugin.Initialize(UserMod.Settings, ForestBrushes.instance.BrushPanel);
            }
        }

        public static T GetPrivate<T>(object o, string fieldName)
        {
            var field = o.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(o);
        }
    }
}
