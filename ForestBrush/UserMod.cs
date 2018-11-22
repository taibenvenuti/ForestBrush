using ColossalFramework;
using ColossalFramework.UI;
using ForestBrush.TranslationFramework;
using Harmony;
using ICities;
using System;
using System.Reflection;
using UnityEngine;

namespace ForestBrush
{
    public class UserMod : IUserMod
    {
        public string Name => "Forest Brush";
        public string Description => Translation.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");
        internal static Translation Translation = new Translation();
        private HarmonyInstance harmony;
        private OptionsKeyBinding optionKeys;

        private static XMLSerialized brushSettings;
        public static XMLSerialized BrushSettings
        {
            get
            {
                if(brushSettings == null)
                {
                    brushSettings = XMLSerialized.Load();
                }
                return brushSettings;
            }
            set
            {
                brushSettings = value;
            }
        }
        public UserMod()
        {
            try
            {
                if (GameSettings.FindSettingsFileByName(CGSSerialized.FileName) == null)
                {
                    GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = CGSSerialized.FileName } });
                    GameSettings.SaveAll();
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("Couldn't find or create the settings file.");
            }
        }

        public void OnEnabled()
        {
            harmony = HarmonyInstance.Create("com.tpb.forestbrush");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;

                UIPanel panel = group.self as UIPanel;

                UICheckBox checkBox = (UICheckBox)group.AddCheckbox(Translation.GetTranslation("FOREST-BRUSH-OPTIONS-CONFIRMOVERWRITE"), CGSSerialized.ConfirmOverwrite, (b) =>
                {

                });

                group.AddSpace(10);

                optionKeys = panel.gameObject.AddComponent<OptionsKeyBinding>();

                group.AddSpace(10);

                UIButton button = (UIButton)group.AddButton(Translation.GetTranslation("FOREST-BRUSH-OPTIONS-RESET"), () =>
                {
                    CGSSerialized.Reset();
                });

                group.AddSpace(10);
            }
            catch (Exception)
            {
                Debug.LogWarning("OnSettingsUI failure.");
            }
        }
    }
}
