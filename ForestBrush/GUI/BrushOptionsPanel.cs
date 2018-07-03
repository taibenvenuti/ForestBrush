using ColossalFramework.UI;
using System;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushOptionsPanel : UIPanel
    {
        UIDragHandle dragHandle;
        UILabel titleLabel;
        UIButton closeButton;
        UILabel densityLabel;
        UISlider densitySlider;
        public UILabel autoDensityLabel;
        public UICheckBox autoDensityCheckBox;
        public UILabel squareBrushLabel;
        public UICheckBox squareBrushCheckBox;

        public override void Start()
        {
            base.Start();
            size = new Vector2(Constants.UIPanelSize.x, Constants.UITitleBarHeight * 2);
            relativePosition = new Vector3(0f, parent.height);
            atlas = UIUtilities.GetAtlas();
            backgroundSprite = "MenuPanel";
            isVisible = false;
            isInteractive = true;          

            titleLabel = AddUIComponent<UILabel>();
            titleLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-TITLE");
            titleLabel.textScale = Constants.UITextScale;
            titleLabel.relativePosition = new Vector3((width - titleLabel.width) / 2f, (Constants.UITitleBarHeight - titleLabel.height) / 2f);

            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(width, Constants.UITitleBarHeight);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            closeButton = AddUIComponent<UIButton>();
            closeButton.atlas = UIUtilities.GetAtlas();
            closeButton.size = new Vector2(20f, 20f);
            closeButton.relativePosition = new Vector3(width - closeButton.width - Constants.UISpacing, Constants.UISpacing);
            closeButton.normalBgSprite = "DeleteLineButton";
            closeButton.hoveredBgSprite = "DeleteLineButtonHovered";
            closeButton.pressedBgSprite = "DeleteLineButtonPressed";
            closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            densityLabel = AddUIComponent<UILabel>();
            densityLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY") + ": " + Math.Round(16f - UserMod.Settings.Spacing, 1).ToString()   ;
            densityLabel.textScale = Constants.UITextScale;

            densitySlider = AddUIComponent<UISlider>();
            densitySlider.size = new Vector2(width - densityLabel.width - Constants.UISpacing * 4, 5f);
            densitySlider.relativePosition = new Vector3(width - densitySlider.width - Constants.UISpacing, Constants.UITitleBarHeight + (Constants.UITitleBarHeight - densitySlider.height) / 2);
            densitySlider.minValue = 0f;
            densitySlider.maxValue = 16f;
            densitySlider.stepSize = 0.1f;
            densitySlider.value = 16 - UserMod.Settings.Spacing;
            densitySlider.scrollWheelAmount = 0.1f;
            densitySlider.eventValueChanged += (c, e) =>
            {
                UserMod.Settings.Spacing = 16f - e;
                densityLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY") + ": " + Math.Round(e, 1).ToString();
                UserMod.Settings.Save();
            };
            densitySlider.backgroundSprite = "SubBarButtonBasePressed";

            UISprite thumb = densitySlider.AddUIComponent<UISprite>();
            thumb.atlas = UIUtilities.GetAtlas();
            thumb.size = new Vector2(20, 20);
            thumb.spriteName = "IconPolicyForest";
            densitySlider.thumbObject = thumb;

            densityLabel.relativePosition = new Vector3(densitySlider.relativePosition.x - densityLabel.width - Constants.UISpacing, Constants.UITitleBarHeight + (Constants.UITitleBarHeight - densityLabel.height) / 2);
            
        }

        public string AutoDensityLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");
        public string SquareBrushLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SQUAREBRUSH");
    }
}
