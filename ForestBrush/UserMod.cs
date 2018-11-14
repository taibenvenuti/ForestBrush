using ForestBrush.TranslationFramework;
using Harmony;
using ICities;
using System.Reflection;

namespace ForestBrush
{
    public class UserMod : IUserMod
    {
        public string Name => "Forest Brushes";
        public string Description => Translation.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");
        internal static Translation Translation = new Translation();
        private HarmonyInstance harmony;
        private static ForestBrushSettings settings;
        public static ForestBrushSettings Settings
        {
            get
            {
                if(settings == null)
                {
                    settings = ForestBrushSettings.Load();
                    if (settings == null)
                    {
                        settings = new ForestBrushSettings();
                        settings.Save();
                    }
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        public void OnEnabled()
        {
            harmony = HarmonyInstance.Create("com.tpb.forestbrush");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
