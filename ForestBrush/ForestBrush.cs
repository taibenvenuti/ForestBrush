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
using Harmony;
using UnityEngine;

namespace ForestBrush
{
    public class ForestBrush : MonoBehaviour
    {
        public static ForestBrush Instance;

        private ToggleButtonComponents toggleButtonComponents;

        public class BrushTweaks
        {
            public int SizeAddend;
            public int SizeMultiplier;
            public float MaxRandomRange;
            public float MinSpacing;
            public float Clearance;
            public int EraserBatchSize;
            public float maxSize;
            public float MaxSize
            {
                get
                {
                    return maxSize;
                }
                set
                {
                    maxSize = value;
                    Instance.ForestBrushPanel.BrushOptionsSection.sizeSlider.maxValue = value;
                }
            }
        }

        internal BrushTweaks BrushTweaker = new BrushTweaks()
        {
            SizeAddend = 10,
            SizeMultiplier = 7,
            MaxRandomRange = 4.0f,
            Clearance = 4.5f,
            maxSize = 1000f,
            EraserBatchSize = 512
        };

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
                }
                return container;
            }
        }

        internal bool IsCurrentTreeContainer => Container != null && ToolsModifierControl.toolController.CurrentTool is TreeTool && ((TreeTool)ToolsModifierControl.toolController?.CurrentTool)?.m_prefab == Container;

        public Dictionary<string, TreeInfo> Trees { get; private set; }

        public Dictionary<string, TreeMeshData> TreesMeshData { get; private set; }

        public Dictionary<string, string> TreeAuthors { get; private set; }

        public ForestBrushTool BrushTool { get; private set; }

        public MethodInfo RayCastMethod = AccessTools.Method(typeof(ToolBase), "RayCast");

        public UIButton ToggleButton => toggleButtonComponents.ToggleButton;

        internal ForestBrushPanel ForestBrushPanel { get; private set; }

        internal bool Initialized;

        internal bool Active => IsCurrentTreeContainer && ForestBrushPanel.isVisible;

        private static readonly string kEmptyContainer = "EmptyContainer";

        private static readonly string kMainToolbarSeparatorTemplate = "MainToolbarSeparator";

        private static readonly string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";

        private static readonly string kToggleButton = "ForestBrushModToggle";

        private ToolBase lastTool;

        private Texture2D lastBrush;

        private float lastSize;

        private TreeTool.Mode lastMode;

        internal void Initialize()
        {
            LoadTrees();

            UITabstrip tabstrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            toggleButtonComponents = CreateToggleButtonComponents(tabstrip);
            ForestBrushPanel = toggleButtonComponents.TabStripPage.GetComponent<UIPanel>().AddUIComponent<ForestBrushPanel>();
            BrushTool = gameObject.AddComponent<ForestBrushTool>();
            SetTutorialLocale();

            toggleButtonComponents.ToggleButton.eventClick += OnToggleClick;
            ForestBrushPanel.eventVisibilityChanged += OnForestBrushPanelVisibilityChanged;
            LocaleManager.eventLocaleChanged += SetTutorialLocale;

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
            TreeTool tool = ToolsModifierControl.GetTool<TreeTool>();

            if (visible)
            {
                ForestBrushPanel.ClampToScreen();
                lastBrush = tool.m_brush;
                lastSize = tool.m_brushSize;
                lastMode = tool.m_mode;
                lastTool = ToolsModifierControl.toolController.CurrentTool;
                ToolsModifierControl.toolController.CurrentTool = tool;
                tool.m_prefab = Container;
                tool.m_brush = ToolsModifierControl.toolController.m_brushes[3];
            }
            else
            {
                tool.m_brush = lastBrush;
                tool.m_brushSize = lastSize;
                tool.m_mode = lastMode;
                if (lastTool != null && lastTool.GetType() != typeof(TreeTool) && ToolsModifierControl.toolController.NextTool == null)
                    lastTool.enabled = true;
            }
        }

        private void OnToggleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            ForestBrushPanel.BringToFront();
            ForestBrushPanel.isVisible = !ForestBrushPanel.isVisible;
        }

        private void LoadTrees()
        {
            Trees = new Dictionary<string, TreeInfo>();
            TreesMeshData = new Dictionary<string, TreeMeshData>();
            var treeCount = PrefabCollection<TreeInfo>.LoadedCount();
            for (uint i = 0; i < treeCount; i++)
            {
                var tree = PrefabCollection<TreeInfo>.GetLoaded(i);
                if (tree == null || tree == Container) continue;
                if (tree.m_availableIn != ItemClass.Availability.All)
                {
                    tree.m_availableIn = ItemClass.Availability.All;
                }

                if (tree.m_Atlas == null || tree.m_Thumbnail.IsNullOrWhiteSpace()) ImageUtils.CreateThumbnailAtlas(GetName(tree), tree);

                Trees.Add(tree.name, tree);
                TreesMeshData.Add(tree.name, new TreeMeshData(tree));
            }
            LoadTreeAuthors();
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
