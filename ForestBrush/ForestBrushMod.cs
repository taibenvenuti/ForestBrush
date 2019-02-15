using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ForestBrush.GUI;
using ForestBrush.Persistence;
using ForestBrush.TranslationFramework;
using UnityEngine;

namespace ForestBrush
{           
    public class ForestBrushMod : Singleton<ForestBrushMod>
    {
        private XmlPersistenceService xmlPersistenceService;
        private Settings settings;

        private ToggleButtonComponents toggleButtonComponents;

        public  class BrushTweaks
        {
            public int SizeAddend;
            public int SizeMultiplier;
            public float MaxRandomRange;
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
                    instance.ForestBrushPanel.BrushOptionsSection.sizeSlider.maxValue = value;
                }
            }
            public float MinSpacing;
        }

        internal BrushTweaks BrushTweaker = new BrushTweaks()
        {
            SizeAddend = 10,
            SizeMultiplier = 7,
            MaxRandomRange = 4.0f,
            maxSize = 1000f
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

        private UITextureAtlas atlas;
        internal UITextureAtlas Atlas
        {
            get
            {
                if (atlas == null)
                {
                    atlas = Resources.ResourceLoader.LoadAtlas();
                }
                return atlas;
            }
        }

        internal bool IsCurrentTreeContainer => Container != null && ToolsModifierControl.toolController.CurrentTool is TreeTool && ((TreeTool)ToolsModifierControl.toolController?.CurrentTool)?.m_prefab == Container;

        public Dictionary<string, TreeInfo> Trees { get; private set; }

        public Settings Settings => settings;

        public ForestBrushTool BrushTool { get; private set; }

        public UIButton ToggleButton => toggleButtonComponents.ToggleButton;

        internal ForestBrushPanel ForestBrushPanel { get; private set; }

        internal bool Initialized;

        private static readonly string kEmptyContainer = "EmptyContainer";

        private static readonly string kMainToolbarSeparatorTemplate = "MainToolbarSeparator";

        private static readonly string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";

        private static readonly string kToggleButton = "ForestBrushModToggle";

        private ToolBase lastTool;

        public void PreInitialize(XmlPersistenceService xmlPersistenceService, Settings settings)
        {
            this.xmlPersistenceService = xmlPersistenceService;
            this.settings = settings;
        }

        internal void Initialize()
        {
            Trees = LoadTrees();

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

            Destroy(BrushTool);
            Destroy(ForestBrushPanel);
            DestroyToggleButtonComponents(toggleButtonComponents);
            toggleButtonComponents = null;

            Trees = null;
            
            settings = null;
            xmlPersistenceService = null;

            Destroy(this);
        }

        private ToggleButtonComponents CreateToggleButtonComponents(UITabstrip tabstrip)
        {
            SeparatorComponents preSeparatorComponents = CreateSeparatorComponents(tabstrip);

            GameObject tabStripPage = UITemplateManager.GetAsGameObject(kEmptyContainer);
            GameObject mainToolbarButtonTemplate = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);

            UIButton toggleButton = tabstrip.AddTab(kToggleButton, mainToolbarButtonTemplate, tabStripPage, new Type[0]) as UIButton;
            toggleButton.atlas = Atlas;

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

            Destroy(toggleButtonComponents.ToggleButton);

            Destroy(toggleButtonComponents.MainToolbarButtonTemplate);
            Destroy(toggleButtonComponents.TabStripPage);

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
            Destroy(separatorComponents.EmptyContainer);
            Destroy(separatorComponents.MainToolbarSeparatorTemplate);
        }

        private void OnForestBrushPanelVisibilityChanged(UIComponent component, bool visible)
        {
            if (visible)
            {
                lastTool = ToolsModifierControl.toolController.CurrentTool;
                TreeTool tool = ToolsModifierControl.GetTool<TreeTool>();
                ToolsModifierControl.toolController.CurrentTool = tool;
                tool.m_prefab = Container;
            }
            else if (lastTool != null && lastTool.GetType() != typeof(TreeTool) && ToolsModifierControl.toolController.NextTool == null)
            {
                lastTool.enabled = true;
            }
        }

        private void OnToggleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            ForestBrushPanel.BringToFront();
            ForestBrushPanel.isVisible = !ForestBrushPanel.isVisible;
        }

        private Dictionary<string, TreeInfo> LoadTrees()
        {
            var trees = new Dictionary<string, TreeInfo>();
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

                trees.Add(tree.name, tree);
            }            
            return trees;
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

                    if (Settings.ToggleTool.IsPressed(e))
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

        public void SaveSettings()
        {
            xmlPersistenceService.Save(Settings);
        }
    }
}
