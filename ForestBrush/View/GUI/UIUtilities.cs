using ColossalFramework.UI;
using ForestBrush.Resources;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class UIUtilities
    {
        public static UIButton CreateButton(UIComponent parentComponent, string text)
        {
            UIButton button = parentComponent.AddUIComponent<UIButton>();
            button.atlas = ResourceLoader.Atlas;
            button.text = text;
            button.width = Constants.UIButtonHeight;
            button.height = Constants.UIButtonHeight;
            button.textPadding = new RectOffset(0, 0, 5, 0);
            button.horizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textScale = Constants.UITitleTextScale;
            button.normalBgSprite = ResourceLoader.ButtonMenu;
            button.disabledBgSprite = ResourceLoader.ButtonMenuDisabled;
            button.hoveredBgSprite = ResourceLoader.ButtonMenuHovered;
            button.focusedBgSprite = ResourceLoader.ButtonMenu;
            button.pressedBgSprite = ResourceLoader.ButtonMenuPressed;
            return button;
        }

        public static UIButton CreateSmallButton(UIComponent parentComponent, string tooltipText)
        {
            UIButton button = parentComponent.AddUIComponent<UIButton>();
            button.atlas = ResourceLoader.Atlas;
            button.normalBgSprite = ResourceLoader.OptionsDropboxListbox;
            button.disabledBgSprite = ResourceLoader.OptionsDropboxListbox;
            button.hoveredBgSprite = ResourceLoader.OptionsDropboxListboxHovered;
            button.focusedBgSprite = ResourceLoader.OptionsDropboxListbox;
            button.pressedBgSprite = ResourceLoader.OptionsDropboxListboxPressed;
            button.tooltip = tooltipText;
            button.size = new Vector2(30f, 30f);
            button.textPadding = new RectOffset(0, 0, 3, 0);
            button.horizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.text = string.Empty;
            button.textScale = 1.2f;
            return button;
        }
    }
}
