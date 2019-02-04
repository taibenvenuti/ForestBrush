using System;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ForestBrush.GUI;
using UnityEngine;

namespace ForestBrush
{           
    public class ForestBrushMod : Singleton<ForestBrushMod>
    {
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

        private CGSSerialized settings;
        internal CGSSerialized Settings
        {
            get
            {
                if (settings == null)
                    settings = new CGSSerialized();
                return settings;
            }
            set
            {
                settings = value;
            }

        }

        private XMLSerialized brushSettings;
        public XMLSerialized BrushSettings
        {
            get
            {
                if (brushSettings == null)
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

        public Dictionary<string, TreeInfo> Trees { get; set; }

        public Dictionary<string, ForestBrush> Brushes { get; set; }

        private ForestBrushTool brushTool;
        public ForestBrushTool BrushTool
        {
            get
            {
                if (brushTool == null)
                    brushTool = gameObject.AddComponent<ForestBrushTool>();
                return brushTool;
            }
            private set
            {
                brushTool = value;
            }
        }

        public UIButton ToggleButton { get; private set; }

        internal ForestBrushPanel ForestBrushPanel { get; private set; }

        internal bool Initialized;

        private static readonly string kEmptyContainer = "EmptyContainer";

        private static readonly string kMainToolbarSeparatorTemplate = "MainToolbarSeparator";

        private static readonly string kMainToolbarButtonTemplate = "MainToolbarButtonTemplate";

        private static readonly string kToggleButton = "ForestBrushModToggle";

        private ToolBase lastTool;

        private GameObject tabStripPage;

        internal void Initialize()
        {
            UITabstrip tabstrip = ToolsModifierControl.mainToolbar.component as UITabstrip;
            
            ToggleButton = CreateToggleButton(tabstrip);

            ForestBrushPanel = tabStripPage.GetComponent<UIPanel>().AddUIComponent<ForestBrushPanel>();
            
            Trees = LoadTrees();

            Brushes = LoadBrushes();

            BrushTool = BrushTool;

            ToggleButton.eventClick += OnToggleClick;

            ForestBrushPanel.eventVisibilityChanged += OnForestBrushPanelVisibilityChanged;

            LocaleManager.eventLocaleChanged += SetTutorialLocale;
            
            SetTutorialLocale();

            Initialized = true;
        }

        private UIButton CreateToggleButton(UITabstrip tabstrip)
        {
            CreateSeparator(tabstrip);

            UIButton button;

            GameObject mainToolbarButtonTemplate = UITemplateManager.GetAsGameObject(kMainToolbarButtonTemplate);

            tabStripPage = UITemplateManager.GetAsGameObject(kEmptyContainer);

            button = tabstrip.AddTab(kToggleButton, mainToolbarButtonTemplate, tabStripPage, new Type[0]) as UIButton;
            button.atlas = Atlas;

            button.normalFgSprite = "ForestBrushNormal";
            button.disabledFgSprite = "ForestBrushDisabled";
            button.focusedFgSprite = "ForestBrushFocused";
            button.hoveredFgSprite = "ForestBrushHovered";
            button.pressedFgSprite = "ForestBrushPressed";

            button.normalBgSprite = "ToolbarIconGroup6Normal";
            button.disabledBgSprite = "ToolbarIconGroup6Disabled";
            button.focusedBgSprite = "ToolbarIconGroup6Focused";
            button.hoveredBgSprite = "ToolbarIconGroup6Hovered";
            button.pressedBgSprite = "ToolbarIconGroup6Pressed";

            IncrementObjectIndex();

            button.parent.height = 1f;

            CreateSeparator(tabstrip);

            return button;
        }

        private void SetTutorialLocale()
        {
            Locale locale = (Locale)typeof(LocaleManager).GetField("m_Locale", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(LocaleManager.instance);
            Locale.Key key = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER_TITLE",
                m_Key = kToggleButton,
            };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, UserMod.Translation.GetTranslation("FOREST-BRUSH-MODNAME"));
            }
            key = new Locale.Key
            {
                m_Identifier = "TUTORIAL_ADVISER",
                m_Key = kToggleButton
            };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, UserMod.Translation.GetTranslation("FOREST-BRUSH-TUTORIAL"));
            }
        }

        private void IncrementObjectIndex()
        {
            FieldInfo m_ObjectIndex = typeof(MainToolbar).GetField("m_ObjectIndex", BindingFlags.Instance | BindingFlags.NonPublic);
            m_ObjectIndex.SetValue(ToolsModifierControl.mainToolbar, (int)m_ObjectIndex.GetValue(ToolsModifierControl.mainToolbar) + 1);
        }

        protected UIComponent CreateSeparator(UITabstrip strip)
        {
            UIComponent uicomponent;
            GameObject asGameObject = UITemplateManager.GetAsGameObject(kMainToolbarSeparatorTemplate);
            GameObject asGameObject2 = UITemplateManager.GetAsGameObject(kEmptyContainer);
            uicomponent = strip.AddTab("Separator", asGameObject, asGameObject2, new Type[0]);
            uicomponent.width *= 0.5f;
            uicomponent.isEnabled = false;
            IncrementObjectIndex();
            return uicomponent;
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

        internal void CleanUp()
        {
            if (ForestBrushPanel && Initialized)
            {
                Destroy(ForestBrushPanel.gameObject);
                ForestBrushPanel = null;
            } 
            Initialized = false;
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

        public Dictionary<string, ForestBrush> LoadBrushes()
        {
            Dictionary<string, ForestBrush> dictionary = new Dictionary<string, ForestBrush>();

            foreach (KeyValuePair<string, ForestBrush> brush in BrushSettings.SavedBrushes)
            {
                if (!dictionary.ContainsKey(brush.Key))
                    dictionary.Add(brush.Key, brush.Value);
            }
            return dictionary;
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
                        ToggleButton.SimulateClick();
                    }

                    if (ForestBrushPanel.isVisible)
                    {

                        if (Settings.Search.IsPressed(e))
                        {
                            ForestBrushPanel.BrushEditSection.FocusSearchField();
                        }
                        if (Settings.ToggleSquare.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.squareBrushCheckBox.SimulateClick();
                        }
                        if (Settings.ToggleAutoDensity.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.autoDensityCheckBox.SimulateClick();
                        }
                        if (Settings.IncreaseSize.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.sizeSlider.value += 5f;
                        }
                        if (Settings.DecreaseSize.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.sizeSlider.value -= 5f;
                        }
                        if (Settings.IncreaseDensity.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.densitySlider.value += 0.1f;
                        }
                        if (Settings.DecreaseDensity.IsPressed(e))
                        {
                            ForestBrushPanel.BrushOptionsSection.densitySlider.value -= 0.1f;
                        }
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
