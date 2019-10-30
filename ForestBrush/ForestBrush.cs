using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using ForestBrush.GUI;
using ForestBrush.TranslationFramework;
using ForestBrush.Persistence;
using UnityEngine;

namespace ForestBrush
{
    public class ForestBrush : MonoBehaviour
    {
        public static ForestBrush Instance;

        private ToggleButtonComponents toggleButtonComponents;

        private TreeInfo container;
        internal TreeInfo Container
        {
            get
            {
                if (container == null)
                {
                    container = Instantiate(PrefabCollection<TreeInfo>.GetLoaded(0u).gameObject).GetComponent<TreeInfo>();
                    container.gameObject.transform.parent = gameObject.transform;
                    container.gameObject.name = "ForestBrushContainer";
                    container.name = "ForestBrushContainer";
                    container.m_mesh = null;
                    container.gameObject.SetActive(false);
                }
                return container;
            }
        }

        internal bool IsCurrentTreeContainer => Container != null && ToolsModifierControl.toolController.CurrentTool is TreeTool && ((TreeTool)ToolsModifierControl.toolController?.CurrentTool)?.m_prefab == Container;

        public Dictionary<string, TreeInfo> Trees { get; private set; }

        public Dictionary<string, TreeMeshData> TreesMeshData { get; private set; }

        public Dictionary<string, string> TreeAuthors { get; private set; }

        public ForestBrushTool BrushTool { get; private set; }

        public UIButton ToggleButton => toggleButtonComponents.ToggleButton;

        public Dictionary<uint, Precision.Data> PrecisionData => Precision.data;
        public HashSet<uint> GrowStateData => GrowState.data;

        internal ForestBrushPanel ForestBrushPanel { get; private set; }

        internal bool Initialized;

        internal bool Active => IsCurrentTreeContainer && ForestBrushPanel.isVisible;

        private static readonly string kEmptyContainer = "EmptyContainer";

        private static readonly string kMainToolbarSeparatorTemplate = "MainToolbarSeparator";

        private static readonly string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";

        private static readonly string kToggleButton = "ForestBrushModToggle";

        public ForestTool Tool { get; private set; }

        private ToolBase lastTool;

        internal void Initialize()
        {
            LoadTrees();
            LoadTreeAuthors();

            UITabstrip tabstrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            toggleButtonComponents = CreateToggleButtonComponents(tabstrip);
            ForestBrushPanel = toggleButtonComponents.TabStripPage.GetComponent<UIPanel>().AddUIComponent<ForestBrushPanel>();
            BrushTool = gameObject.AddComponent<ForestBrushTool>();
            BrushTool.UpdateTool(UserMod.Settings.SelectedBrush.Name);
            SetTutorialLocale();

            toggleButtonComponents.ToggleButton.eventClick += OnToggleClick;
            ForestBrushPanel.eventVisibilityChanged += OnForestBrushPanelVisibilityChanged;
            LocaleManager.eventLocaleChanged += SetTutorialLocale;

            Tool = ToolsModifierControl.toolController.gameObject.AddComponent<ForestTool>();

            Initialized = true;
        }

        internal void CleanUp()
        {
            Initialized = false;

            LocaleManager.eventLocaleChanged -= SetTutorialLocale;
            ForestBrushPanel.eventVisibilityChanged -= OnForestBrushPanelVisibilityChanged;
            toggleButtonComponents.ToggleButton.eventClick -= OnToggleClick;

            Destroy(BrushTool.gameObject);
            Destroy(ForestBrushPanel.gameObject);
            DestroyToggleButtonComponents(toggleButtonComponents);
            toggleButtonComponents = null;
            TreesMeshData = null;
            Trees = null;
        }

        private ToggleButtonComponents CreateToggleButtonComponents(UITabstrip tabstrip)
        {
            SeparatorComponents preSeparatorComponents = CreateSeparatorComponents(tabstrip);

            GameObject tabStripPage = UITemplateManager.GetAsGameObject(kEmptyContainer);
            GameObject mainToolbarButtonTemplate = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);

            UIButton toggleButton = tabstrip.AddTab(kToggleButton, mainToolbarButtonTemplate, tabStripPage, new Type[0]) as UIButton;
            toggleButton.atlas = Resources.ResourceLoader.ForestBrushAtlas;

            toggleButton.normalFgSprite = "ForestBrushNormal";
            toggleButton.disabledFgSprite = "ForestBrushDisabled";
            toggleButton.focusedFgSprite = "ForestBrushFocused";
            toggleButton.hoveredFgSprite = "ForestBrushHovered";
            toggleButton.pressedFgSprite = "ForestBrushPressed";

            toggleButton.normalBgSprite = "ToolbarIconGroup6Normal";
            toggleButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
            toggleButton.focusedBgSprite = "ToolbarIconGroup6Focused";
            toggleButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
            toggleButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
            toggleButton.parent.height = 1f;

            IncrementObjectIndex();

            SeparatorComponents postSeparatorComponents = CreateSeparatorComponents(tabstrip);

            return new ToggleButtonComponents(preSeparatorComponents, tabStripPage, mainToolbarButtonTemplate, toggleButton, postSeparatorComponents);
        }

        private void DestroyToggleButtonComponents(ToggleButtonComponents toggleButtonComponents)
        {
            DestroySeparatorComponents(toggleButtonComponents.PostSeparatorComponents);

            DecrementObjectIndex();

            Destroy(toggleButtonComponents.ToggleButton.gameObject);

            Destroy(toggleButtonComponents.MainToolbarButtonTemplate.gameObject);
            Destroy(toggleButtonComponents.TabStripPage.gameObject);

            DestroySeparatorComponents(toggleButtonComponents.PreSeparatorComponents);
        }

        private void SetTutorialLocale()
        {
            Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(LocaleManager.instance);

            Locale.Key tutorialAdviserTitleKey = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER_TITLE",
                m_Key = kToggleButton,
            };
            if (!locale.Exists(tutorialAdviserTitleKey))
            {
                locale.AddLocalizedString(tutorialAdviserTitleKey, Translation.Instance.GetTranslation("FOREST-BRUSH-MODNAME"));
            }

            Locale.Key tutorialAdviserKey = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER",
                m_Key = kToggleButton
            };
            if (!locale.Exists(tutorialAdviserKey))
            {
                locale.AddLocalizedString(tutorialAdviserKey, Translation.Instance.GetTranslation("FOREST-BRUSH-TUTORIAL"));
            }
        }

        private void IncrementObjectIndex()
        {
            FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) + 1);
        }

        private void DecrementObjectIndex()
        {
            FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) - 1);
        }

        protected SeparatorComponents CreateSeparatorComponents(UITabstrip strip)
        {
            GameObject mainToolbarSeparatorTemplate = UITemplateManager.GetAsGameObject(kMainToolbarSeparatorTemplate);
            GameObject emptyContainer = UITemplateManager.GetAsGameObject(kEmptyContainer);
            UIComponent separatorTab = strip.AddTab("Separator", mainToolbarSeparatorTemplate, emptyContainer, new Type[0]);
            separatorTab.width *= 0.5f;
            separatorTab.isEnabled = false;
            IncrementObjectIndex();
            return new SeparatorComponents(mainToolbarSeparatorTemplate, emptyContainer, separatorTab);
        }

        protected void DestroySeparatorComponents(SeparatorComponents separatorComponents)
        {
            DecrementObjectIndex();
            Destroy(separatorComponents.SeparatorTab.gameObject);
            Destroy(separatorComponents.EmptyContainer.gameObject);
            Destroy(separatorComponents.MainToolbarSeparatorTemplate.gameObject);
        }

        private void OnForestBrushPanelVisibilityChanged(UIComponent component, bool visible)
        {
            if (visible)
            {
                ForestBrushPanel.ClampToScreen();
                lastTool = ToolsModifierControl.toolController.CurrentTool;
                ToolsModifierControl.SetTool<ForestTool>();
            }
            else
            {
                if (lastTool != null && lastTool.GetType() != typeof(TreeTool) && ToolsModifierControl.toolController.NextTool == null)
                    lastTool.enabled = true;
            }
        }

        private void OnToggleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            ForestBrushPanel.BringToFront();
            ForestBrushPanel.isVisible = !ForestBrushPanel.isVisible;
        }

        internal void LoadTrees()
        {
            Trees = new Dictionary<string, TreeInfo>();
            TreesMeshData = new Dictionary<string, TreeMeshData>();
            var treeCount = PrefabCollection<TreeInfo>.LoadedCount();
            for (uint i = 0; i < treeCount; i++)
            {
                var tree = PrefabCollection<TreeInfo>.GetLoaded(i);
                if (tree == null || tree == Container || (UserMod.Settings != null && UserMod.Settings.IgnoreVanillaTrees && !tree.m_isCustomContent)) continue;
                if (tree.m_availableIn != ItemClass.Availability.All)
                {
                    tree.m_availableIn = ItemClass.Availability.All;
                }

                if (tree.m_Atlas == null || string.IsNullOrEmpty(tree.m_Thumbnail) || tree.m_Thumbnail.IsNullOrWhiteSpace()) ImageUtils.CreateThumbnailAtlas(GetName(tree), tree);

                Trees.Add(tree.name, tree);
                TreesMeshData.Add(tree.name, new TreeMeshData(tree));
            }
        }

        private void LoadTreeAuthors()
        {
            TreeAuthors = new Dictionary<string, string>();
            foreach (Package.Asset current in PackageManager.FilterAssets(new Package.AssetType[] { UserAssetType.CustomAssetMetaData }))
            {
                PublishedFileId id = current.package.GetPublishedFileID();
                string publishedFileId = string.Concat(id.AsUInt64);
                if (!TreeAuthors.ContainsKey(publishedFileId) && !current.package.packageAuthor.IsNullOrWhiteSpace())
                {
                    if (ulong.TryParse(current.package.packageAuthor.Substring("steamid:".Length), out ulong authorID))
                    {
                        string author = new Friend(new UserID(authorID)).personaName;
                        TreeAuthors.Add(publishedFileId, author);
                    }
                }
            }
        }

        public static string GetName(PrefabInfo prefab)
        {
            string name = prefab.name;
            if (name.EndsWith("_Data"))
            {
                name = name.Substring(0, name.LastIndexOf("_Data"));
            }
            return name;
        }

        public void SetBrush(string id)
        {
            Tool.SetBrush(id);
        }

        public Dictionary<string, Texture2D> GetBrushBitmaps()
        {
            return Tool.GetBrushes();
        }

        public void OnGUI()
        {
            try
            {
                if (Initialized && !UIView.HasModalInput() &&
                    (!UIView.HasInputFocus() || (UIView.activeComponent != null)))
                {
                    Event e = Event.current;

                    if (UserMod.Settings.ToggleTool.IsPressed(e))
                    {
                        toggleButtonComponents.ToggleButton.SimulateClick();
                    }
                }
                
            }
            catch (Exception)
            {
                Debug.LogWarning("OnGUI failed.");
            }
        }
    }
}
