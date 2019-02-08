using ColossalFramework;
using ColossalFramework.UI;
using ForestBrush.TranslationFramework;
using ICities;
using System;
using UnityEngine;

namespace ForestBrush
{
    public class UserMod : IUserMod
    {
        public string Name => "Forest Brush";
        public string Description => Translation.Instance.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");

        private OptionsKeyBinding optionKeys;

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

        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;

                UIPanel panel = group.self as UIPanel;

                group.AddSpace(10);

                optionKeys = panel.gameObject.AddComponent<OptionsKeyBinding>();

                group.AddSpace(10);

                UIButton button = (UIButton)group.AddButton(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-RESET"), () =>
                {
                    ForestBrushMod.instance.Settings.Reset();
                    optionKeys.RefreshBindableInputs();
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
