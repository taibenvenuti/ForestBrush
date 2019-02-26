using ColossalFramework;
using ColossalFramework.UI;
using ForestBrush.Persistence;
using ForestBrush.TranslationFramework;
using Harmony;
using ICities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ForestBrush
{
    public class UserMod : LoadingExtensionBase, IUserMod
    {
        //out of game dependencies
        public static XmlPersistenceService XmlPersistenceService { get; private set; }
        public static Settings Settings { get; private set; }
        public static LoadMode LoadMode;
        //in game dependecies
        private readonly string harmonyId = "com.tpb.forestbrush";
        private HarmonyInstance harmony;
        private bool modInstalled = false;
        private OptionsKeyBinding optionKeys;
        private GameObject forestBrushGameObject;
        private UIDropDown searchLogic;
        private UIDropDown newBrushBehaviour;

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

        public static bool IsGame = LoadMode == LoadMode.LoadGame || LoadMode == LoadMode.NewGame || LoadMode == LoadMode.NewGameFromScenario;
        public static bool IsMap = LoadMode == LoadMode.LoadMap || LoadMode == LoadMode.NewMap;
        public static bool IsTheme = LoadMode == LoadMode.NewTheme || LoadMode == LoadMode.LoadTheme;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            LoadMode = mode;
            if (IsGame || IsMap || IsTheme)
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
            GameSettings.AddSettingsFile(new SettingsFile { fileName = Constants.ModName });
            XmlPersistenceService = new XmlPersistenceService();
            Settings = XmlPersistenceService.Load();
        }

        void UninstallOutOfGameDependencies()
        {
            XmlPersistenceService = null;
            Settings = null;
            var settingFiles = (Dictionary<string, SettingsFile>)typeof(GameSettings).GetField("m_SettingsFiles", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(GameSettings.instance);
            settingFiles.Remove(Constants.ModName);
        }

        void InstallMod()
        {
            if (forestBrushGameObject != null)
            {
                UninstallMod();
            }
            if (IsMap || IsTheme) Resources.ResourceLoader.Atlas = Resources.ResourceLoader.Atlas;
            Resources.ResourceLoader.ForestBrushAtlas = Resources.ResourceLoader.ForestBrushAtlas;
            harmony = HarmonyInstance.Create(harmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            forestBrushGameObject = new GameObject("ForestBrush");
            ForestBrush.Instance = forestBrushGameObject.AddComponent<ForestBrush>();
            ForestBrush.Instance.Initialize();

            modInstalled = true;
        }

        void UninstallMod()
        {
            try { harmony.UnpatchAll(harmonyId); }
            catch (Exception) { }
            finally { harmony = null; }

            ForestBrush.Instance.CleanUp();

            if (IsMap || IsTheme)
            {
                if(Resources.ResourceLoader.Atlas != null) UnityEngine.Resources.UnloadAsset(Resources.ResourceLoader.Atlas);
            }
            if (forestBrushGameObject != null)
            {
                GameObject.Destroy(forestBrushGameObject);
            }
            modInstalled = false;
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;

                UIPanel panel = group.self as UIPanel;

                group.AddSpace(10);

                group.AddDropdown(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-SORTING"), 
                                  new string[] 
                                  {
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-DATA-NAME"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-DATA-AUTHOR"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-DATA-TEXTURE"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-DATA-TRIANGLES")
                                  }, 
                                  (int)Settings.Sorting, 
                                  (index) => 
                                  {
                                      Settings.Sorting = (TreeSorting)index; SaveSettings();
                                  });

                group.AddSpace(10);

                group.AddDropdown(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-SORTING-ORDER"),
                                  new string[]
                                  {
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-SORTING-DESCENDING"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-SORTING-ASCENDING")
                                  },
                                  (int)Settings.SortingOrder,
                                  (index) =>
                                  {
                                      Settings.SortingOrder = (SortingOrder)index; SaveSettings();
                                  });

                group.AddSpace(10);

                searchLogic = (UIDropDown)group.AddDropdown(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-FILTERING-LOGIC"),
                                  new string[]
                                  {
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-FILTERING-AND"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-FILTERING-OR"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-FILTERING-SIMPLE")
                                  },
                                  (int)Settings.FilterStyle,
                                  (index) =>
                                  {
                                      Settings.FilterStyle = (FilterStyle)index; SaveSettings();
                                  });
                searchLogic.width = 500.0f;

                group.AddSpace(10);

                newBrushBehaviour = (UIDropDown)group.AddDropdown(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-NEWBRUSH"),
                                  new string[]
                                  {
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-NEWBRUSH-CLEAR"),
                                      Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-NEWBRUSH-KEEP")
                                  },
                                  Settings.KeepTreesInNewBrush ? 1 : 0,
                                  (index) =>
                                  {
                                      Settings.KeepTreesInNewBrush = index == 1 ? true : false; SaveSettings();
                                  });
                newBrushBehaviour.width = 500.0f;

                group.AddSpace(10);

                group.AddCheckbox(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-SHOWMESHDATA"), Settings.ShowTreeMeshData, (b) => { Settings.ShowTreeMeshData = b; SaveSettings(); });

                group.AddSpace(10);

                optionKeys = panel.gameObject.AddComponent<OptionsKeyBinding>();

                group.AddSpace(10);
            }
            catch (Exception)
            {
                Debug.LogWarning("OnSettingsUI failure.");
            }
        }

        public static void SaveSettings()
        {
            XmlPersistenceService.Save(Settings);
        }
    }
}
