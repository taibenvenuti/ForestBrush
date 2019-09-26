using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ForestBrush.Persistence;
using ForestBrush.Redirection;
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
        private bool modInstalled = false;
        private OptionsKeyBinding optionKeys;
        private GameObject forestBrushGameObject;
        private UIDropDown searchLogic;
        private UIDropDown newBrushBehaviour;
        private HarmonyInstance Harmony { get; set; }
        private const string HarmonyID = "com.tpb.forestbrush";

        public string Name => Constants.ModName;

        public string Description => Translation.Instance.GetTranslation("FOREST-BRUSH-MODDESCRIPTION");

        public static bool IsModEnabled(ulong publishedFileID, string modName) {
            foreach (var plugin in PluginManager.instance.GetPluginsInfo()) {
                if (plugin.publishedFileID.AsUInt64 == publishedFileID
                    || plugin.name == modName) {
                    return plugin.isEnabled;
                }
            }
            return false;
        }

        public void OnEnabled()
        {
            Patch();
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
            Unpatch();
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
            if (GameSettings.FindSettingsFileByName(Constants.ModName) == null) {
                GameSettings.AddSettingsFile(new SettingsFile { fileName = Constants.ModName });
            }
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

        private void Patch() {
            if (!IsModEnabled(593588108UL, "Prop & Tree Anarchy")) {
                if (Harmony == null) {
                    Harmony = HarmonyInstance.Create(HarmonyID);
                    Harmony.PatchAll(Assembly.GetExecutingAssembly());

                }
            }
            if (!IsModEnabled(1873351912UL, "Tree Precision")) {
                Redirector<TreeInstanceDetour>.Deploy();
            }
        }

        private void Unpatch() {
            if (Harmony != null) {
                try {
                    Harmony.UnpatchAll(HarmonyID);
                } catch (Exception) {
                    Debug.LogWarning($"Failed unpatching {HarmonyID}, please notify the mod author.");
                } finally {
                    Harmony = null;
                }
            }
            Redirector<TreeInstanceDetour>.Revert();
        }

        void InstallMod()
        {
            if (forestBrushGameObject != null)
            {
                UninstallMod();
            }
            if (!IsGame && !IsMap && !IsTheme) return;
            if (IsMap || IsTheme) Resources.ResourceLoader.Atlas = Resources.ResourceLoader.Atlas;
            Resources.ResourceLoader.ForestBrushAtlas = Resources.ResourceLoader.ForestBrushAtlas;
            forestBrushGameObject = new GameObject("ForestBrush");
            ForestBrush.Instance = forestBrushGameObject.AddComponent<ForestBrush>();
            ForestBrush.Instance.Initialize();

            modInstalled = true;
        }

        void UninstallMod()
        {
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

                group.AddCheckbox(Translation.Instance.GetTranslation("FOREST-BRUSH-OPTIONS-IGNORE-VANILLA"), Settings.IgnoreVanillaTrees, (b) => 
                {
                    Settings.IgnoreVanillaTrees = b;
                    SaveSettings();
                    if (LoadingManager.instance.m_loadingComplete)
                    {
                        ForestBrush.Instance.LoadTrees();
                        ForestBrush.Instance.ForestBrushPanel.BrushEditSection.SetupFastlist();
                    }
                });

                group.AddSpace(20);

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
