using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class UIUtilities
    {
        private static Dictionary<string, UITextureAtlas> _atlases;

        public static UITextureAtlas GetAtlas()
        {
            if (_atlases == null || !_atlases.ContainsKey("Ingame"))
            {
                _atlases = new Dictionary<string, UITextureAtlas>();

                UITextureAtlas[] atlases = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
                for (int i = 0; i < atlases.Length; i++)
                {
                    if (!_atlases.ContainsKey(atlases[i].name))
                        _atlases.Add(atlases[i].name, atlases[i]);
                }
            }

            return _atlases["Ingame"];
        }

        public static UIButton CreateButton(UIComponent parentComponent, string text)
        {
            UIButton button = parentComponent.AddUIComponent<UIButton>();
            button.atlas = GetAtlas();
            button.text = text;
            button.width = Constants.UIButtonWidth;
            button.height = Constants.UIButtonHeight;
            button.textPadding = new RectOffset(0, 0, 5, 0);
            button.horizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textScale = Constants.UITextScale;
            button.normalBgSprite = "ButtonMenu";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.focusedBgSprite = "ButtonMenu";
            button.pressedBgSprite = "ButtonMenuPressed";
            return button;
        }
    }
}
