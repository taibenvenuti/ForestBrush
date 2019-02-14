using ColossalFramework.UI;
using ForestBrush.Persistence;
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
        //out of game dependencies
        private XmlPersistenceService xmlPersistenceService;
        private Settings settings;

        //in game dependecies
        private readonly string harmonyId = "com.tpb.forestbrush";
        private HarmonyInstance harmony;
        private bool modInstalled = false;

        public string Name => Constants.ModName;

        public string Description => Translation.Instance.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");

        public void OnEnabled()
        {
            InstallOutOfGameDependencies();

            if (LoadingManager.exists && LoadingManager.instance.m_loadingComplete)
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

            UninstallOutOfGameDependencies();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame || mode == LoadMode.NewGameFromScenario)
            {
                InstallMod();
            }
        }

        public override void OnLevelUnloading()
        {
            if (modInstalled)
            {
                UninstallMod();
            }

            base.OnLevelUnloading();
        }

        void InstallOutOfGameDependencies()
        {
            xmlPersistenceService = new XmlPersistenceService();
            settings = xmlPersistenceService.Load();
            ForestBrushMod.instance.PreInitialize(xmlPersistenceService, settings);
        }

        void UninstallOutOfGameDependencies()
        {
            xmlPersistenceService = null;
            settings = null;
        }

        void InstallMod()
        {
            harmony = HarmonyInstance.Create(harmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ForestBrushMod.instance.Initialize();

            modInstalled = true;
        }

        void UninstallMod()
        {
            ForestBrushMod.instance.CleanUp();

            harmony.UnpatchAll(harmonyId);
            harmony = null;

            modInstalled = false;
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;

                UIPanel panel = group.self as UIPanel;

                group.AddSpace(10);

                var optionKeys = panel.gameObject.AddComponent<OptionsKeyBinding>();

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
    }
}
