using ColossalFramework.UI;
using ForestBrush.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushShapeSelector : UIPanel
    {
        ForestBrushPanel owner;
        private Dictionary<string, UIButton> BrushButtons = new Dictionary<string, UIButton>();
        private UIScrollablePanel ScrollablePanel;

        public override void Start()
        {
            base.Start();
            Setup();

            LoadBrush(UserMod.Settings.SelectedBrush);

            if (!UserMod.Settings.BrushShapesOpen)
            {
                owner.BrushSelectSection.UnfocusShapesSectionButton();
                Hide();
            }
        }

        private void Setup()
        {
            width = parent.width;
            height = 122f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            padding = new RectOffset(10, 0, 0, 0);
            autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            isInteractive = true;
            builtinKeyNavigation = true;
            relativePosition = new Vector2(410.0f, 0.0f);

            ScrollablePanel = AddUIComponent<UIScrollablePanel>();
            ScrollablePanel.zOrder = 0;
            ScrollablePanel.width = 366f;
            ScrollablePanel.height = height;
            ScrollablePanel.autoLayout = true;
            ScrollablePanel.wrapLayout = true;
            ScrollablePanel.clipChildren = true;
            ScrollablePanel.backgroundSprite = "TextFieldPanel";
            ScrollablePanel.autoLayoutDirection = LayoutDirection.Horizontal;
            ScrollablePanel.autoLayoutPadding = new RectOffset(2, 2, 2, 2);
            ScrollablePanel.color = new Color32(255, 255, 255, 255);
            ScrollablePanel.isInteractive = true;
            ScrollablePanel.builtinKeyNavigation = true;
            ScrollablePanel.scrollWheelDirection = UIOrientation.Vertical;
            ScrollablePanel.scrollWithArrowKeys = true;
            Dictionary<string, Texture2D> brushes = ForestBrush.Instance.GetBrushBitmaps();
            foreach (KeyValuePair<string, Texture2D> brush in brushes)
            {
                UIButton button = ScrollablePanel.AddUIComponent<UIButton>();
                button.atlas = ResourceLoader.Atlas;
                button.normalBgSprite = ResourceLoader.GenericPanel;
                if (UserMod.Settings.SelectedBrush != null 
                && UserMod.Settings.SelectedBrush.Options != null 
                && UserMod.Settings.SelectedBrush.Options.BitmapID == brush.Key)
                    button.normalBgSprite = string.Empty;
                button.color = new Color32(170, 170, 170, 255);
                button.hoveredColor = new Color32(210, 210, 210, 255);
                button.focusedColor = Color.white;
                button.pressedColor = Color.white;
                button.size = new Vector2(57.0f, 57.0f);
                UITextureSprite sprite = button.AddUIComponent<UITextureSprite>();
                sprite.size = button.size - new Vector2(4.0f, 4.0f);
                sprite.relativePosition = new Vector2(2.0f, 2.0f);
                sprite.texture = brush.Value;
                button.objectUserData = brush.Key;
                button.eventClicked += Button_eventClicked;
                BrushButtons.Add(brush.Key, button);
            }

            UIScrollbar scrollbar = AddUIComponent<UIScrollbar>();
            scrollbar.zOrder = 1;
            scrollbar.width = 13f;
            scrollbar.height = ScrollablePanel.height;
            scrollbar.orientation = UIOrientation.Vertical;
            scrollbar.minValue = 0;
            scrollbar.value = 0;
            scrollbar.incrementAmount = 61;

            UISlicedSprite tracSprite = scrollbar.AddUIComponent<UISlicedSprite>();
            tracSprite.atlas = ResourceLoader.Atlas;
            tracSprite.relativePosition = new Vector3(0, 0);
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Vertical;
            tracSprite.spriteName = ResourceLoader.LevelBarBackground;

            scrollbar.trackObject = tracSprite;

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.atlas = ResourceLoader.Atlas;
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = 13f;
            thumbSprite.spriteName = ResourceLoader.LevelBarForeground;

            scrollbar.thumbObject = thumbSprite;

            ScrollablePanel.verticalScrollbar = scrollbar;

            owner = (ForestBrushPanel)parent;
        }

        internal void LoadBrush(Brush brush)
        {
            ResetButtons();
                
            if (BrushButtons.TryGetValue(brush.Options.BitmapID, out UIButton button))
            {
                button.normalBgSprite = string.Empty;
                ForestBrush.Instance.SetBrush((string)button.objectUserData);
            }
            else
            {
                var buttonkvp = BrushButtons.FirstOrDefault(b => b.Value != null);
                buttonkvp.Value.normalBgSprite = string.Empty;
                ForestBrush.Instance.SetBrush((string)buttonkvp.Value.objectUserData);
            }
        }

        private void Button_eventClicked(UIComponent comp, UIMouseEventParameter eventParam)
        {
            ResetButtons();
            UIButton caller = comp as UIButton;
            caller.normalBgSprite = string.Empty;
            ForestBrush.Instance.SetBrush((string)caller.objectUserData);
        }

        private void ResetButtons()
        {
            foreach (var button in BrushButtons.Values)
            {
                if (button == null) continue;
                button.normalBgSprite = ResourceLoader.GenericPanel;
            }
        }
    }
}
