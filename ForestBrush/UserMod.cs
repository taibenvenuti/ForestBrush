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
    public class UserMod : LoadingExtensionBase, IUserMod
    {
        private OptionsKeyBinding optionKeys;
        private HarmonyInstance harmony;
        private readonly string harmonyId = "com.tpb.forestbrush";

        public string Name => Constants.ModName;

        public string Description => Translation.Instance.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");

        public void OnEnabled()
        {
            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
                InstallMod();
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario)
            {
                InstallMod();
            }
        }

        public void OnDisabled()
        {
            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
            {
                UninstallMod();
            }
        }

        public override void OnLevelUnloading()
        {
            UninstallMod();

            base.OnLevelUnloading();
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
                    ForestBrushMod.instance.SaveSettings();
                });

                group.AddSpace(10);
            }
            catch (Exception)
            {
                Debug.LogWarning("OnSettingsUI failure.");
            }
        }

        void InstallMod()
        {
            harmony = HarmonyInstance.Create(harmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ForestBrushMod.instance.Initialize();
        }

        void UninstallMod()
        {
            ForestBrushMod.instance.CleanUp();

            harmony.UnpatchAll(harmonyId);
            harmony = null;
        }
    }
}
