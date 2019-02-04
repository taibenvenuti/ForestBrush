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
            button.atlas = ResourceLoader.GetAtlas("Ingame");
            button.text = text;
            button.width = Constants.UIButtonHeight;
            button.height = Constants.UIButtonHeight;
            button.textPadding = new RectOffset(0, 0, 5, 0);
            button.horizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textScale = Constants.UITitleTextScale;
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenu";
            button.pressedBgSprite = "ButtonMenuPressed";
            return button;
        }

        public static UIButton CreateSmallButton(UIComponent parentComponent, string tooltipText)
        {
            UIButton button = parentComponent.AddUIComponent<UIButton>();
            button.atlas = ResourceLoader.GetAtlas("Ingame");
            button.normalBgSprite = "OptionsDropboxListbox";
            button.disabledBgSprite = "OptionsDropboxListbox";
            button.hoveredBgSprite = "OptionsDropboxListboxHovered";
            button.focusedBgSprite = "OptionsDropboxListbox";
            button.pressedBgSprite = "OptionsDropboxListboxPressed";
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
