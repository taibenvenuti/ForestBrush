using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class TitleSection : UIPanel
    {
        UISprite icon;
        UIDragHandle dragHandle;
        UILabel titleLabel;
        UIButton closeButton;

        public override void Start()
        {
            base.Start();

            width = parent.width;
            height = Constants.UITitleBarHeight;

            icon = AddUIComponent<UISprite>();
            icon.atlas = ForestBrushMod.instance.Atlas;
            icon.spriteName = ResourceLoader.ForestBrushNormal;
            icon.relativePosition = new Vector3(5f, 5f);
            icon.size = new Vector2(28f, 32f);

            titleLabel = AddUIComponent<UILabel>();
            titleLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-MODNAME");
            titleLabel.textScale = Constants.UITitleTextScale;
            titleLabel.relativePosition = new Vector3((width - titleLabel.width) / 2f, (Constants.UITitleBarHeight - titleLabel.height) / 2f);

            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(width, Constants.UITitleBarHeight);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = parent;
            dragHandle.eventMouseUp += (c, e) => SavePanelPosition();

            closeButton = AddUIComponent<UIButton>();
            closeButton.atlas = ResourceLoader.GetAtlas("Ingame");
            closeButton.size = new Vector2(20f, 20f);
            closeButton.relativePosition = new Vector3(width - closeButton.width - Constants.UISpacing, Constants.UISpacing);
            closeButton.normalBgSprite = "DeleteLineButton";
            closeButton.hoveredBgSprite = "DeleteLineButtonHovered";
            closeButton.pressedBgSprite = "DeleteLineButtonPressed";
            closeButton.eventClick += (c, p) => ForestBrushMod.instance.ToggleButton.SimulateClick();
        }

        public void SavePanelPosition()
        {
            ForestBrushMod.instance.Settings.PanelX.value = parent.absolutePosition.x;
            ForestBrushMod.instance.Settings.PanelY.value = parent.absolutePosition.y;
        }
    }
}
