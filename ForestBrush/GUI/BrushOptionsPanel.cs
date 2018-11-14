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
        public UILabel overlayColorLabel;
        public UIColorField colorFieldTemplate;
        public UIPanel layoutPanel0;
        public UIPanel layoutPanel1;
        public UIPanel layoutPanel2;
        public UIPanel layoutPanel3;
        public UIPanel layoutPanel4;

        public override void Start()
        {
            base.Start();

            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 0, 14);


            width = 400f;
            relativePosition = new Vector3(0f, parent.height);
            atlas = UIUtilities.GetAtlas();
            backgroundSprite = "MenuPanel";
            isVisible = false;
            isInteractive = true;


            layoutPanel0 = AddUIComponent<UIPanel>();
            layoutPanel0.size = new Vector2(width, 40);
            layoutPanel0.zOrder = 0;

            titleLabel = layoutPanel0.AddUIComponent<UILabel>();
            titleLabel.autoSize = titleLabel.autoHeight = false;
            titleLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-TITLE");
            titleLabel.textScale = Constants.UITextScale;
            titleLabel.verticalAlignment = UIVerticalAlignment.Middle;
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.size = new Vector2(layoutPanel0.width, 40f);
            titleLabel.relativePosition = Vector3.zero;

            dragHandle = layoutPanel0.AddUIComponent<UIDragHandle>();
            dragHandle.size = titleLabel.size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            closeButton = layoutPanel0.AddUIComponent<UIButton>();
            closeButton.atlas = UIUtilities.GetAtlas();
            closeButton.size = new Vector2(20f, 20f);
            closeButton.relativePosition = new Vector3(width - 30f, 10f);
            closeButton.normalBgSprite = "DeleteLineButton";
            closeButton.hoveredBgSprite = "DeleteLineButtonHovered";
            closeButton.pressedBgSprite = "DeleteLineButtonPressed";
            closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            layoutPanel1 = AddUIComponent<UIPanel>();
            layoutPanel1.size = new Vector2(width, 16);
            layoutPanel1.autoLayout = true;
            layoutPanel1.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanel1.autoFitChildrenHorizontally = true;
            layoutPanel1.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanel1.zOrder = 1;

            densityLabel = layoutPanel1.AddUIComponent<UILabel>();
            densityLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY") + ": " + Math.Round(16f - UserMod.Settings.Spacing, 1).ToString();
            densityLabel.textScale = Constants.UITextScale;
            densityLabel.autoSize = true;
            densityLabel.textAlignment = UIHorizontalAlignment.Left;
            densityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            densityLabel.zOrder = 0;

            densitySlider = layoutPanel1.AddUIComponent<UISlider>();
            densitySlider.size = new Vector2(400f - densityLabel.size.x - 42f, 13f);
            densitySlider.color = new Color32(0, 0, 0, 128);
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
            densitySlider.backgroundSprite = "OptionsScrollbarTrack";
            densitySlider.zOrder = 1;

            UISprite thumb = densitySlider.AddUIComponent<UISprite>();
            thumb.atlas = UIUtilities.GetAtlas();
            thumb.size = new Vector2(20, 20);
            thumb.spriteName = "IconPolicyForest";
            densitySlider.thumbObject = thumb;

            
            layoutPanel2 = AddUIComponent<UIPanel>();
            layoutPanel2.size = new Vector2(width, 16);
            layoutPanel2.autoLayout = true;
            layoutPanel2.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanel2.autoFitChildrenHorizontally = true;
            layoutPanel2.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanel2.zOrder = 2;

            autoDensityLabel = layoutPanel2.AddUIComponent<UILabel>();
            autoDensityLabel.text = AutoDensityLabelText;
            autoDensityLabel.textScale = Constants.UITextScale;
            autoDensityLabel.autoSize = true;
            autoDensityLabel.textAlignment = UIHorizontalAlignment.Left;
            autoDensityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            autoDensityLabel.zOrder = 0;

            autoDensityCheckBox = layoutPanel2.AddUIComponent<UICheckBox>();
            autoDensityCheckBox.size = Constants.UICheckboxSize;
            var sprite = autoDensityCheckBox.AddUIComponent<UISprite>();
            sprite.atlas = UIUtilities.GetAtlas();
            sprite.spriteName = "ToggleBase";
            sprite.size = autoDensityCheckBox.size;
            sprite.transform.parent = autoDensityCheckBox.transform;
            sprite.transform.localPosition = Vector3.zero;
            autoDensityCheckBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)autoDensityCheckBox.checkedBoxObject).atlas = UIUtilities.GetAtlas();
            ((UISprite)autoDensityCheckBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            autoDensityCheckBox.checkedBoxObject.size = autoDensityCheckBox.size;
            autoDensityCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            autoDensityCheckBox.isChecked = UserMod.Settings.UseTreeSize;
            autoDensityCheckBox.eventCheckChanged += (c, e) =>
            {
                UserMod.Settings.UseTreeSize = e;
                UserMod.Settings.Save();
            };
            autoDensityCheckBox.zOrder = 1;

            squareBrushLabel = layoutPanel2.AddUIComponent<UILabel>();
            squareBrushLabel.text = SquareBrushLabelText;
            squareBrushLabel.textScale = Constants.UITextScale;
            squareBrushLabel.autoSize = true;
            squareBrushLabel.textAlignment = UIHorizontalAlignment.Left;
            squareBrushLabel.verticalAlignment = UIVerticalAlignment.Middle;
            squareBrushLabel.zOrder = 2;

            squareBrushCheckBox = layoutPanel2.AddUIComponent<UICheckBox>();
            squareBrushCheckBox.size = Constants.UICheckboxSize;
            var sprite2 = squareBrushCheckBox.AddUIComponent<UISprite>();
            sprite2.atlas = UIUtilities.GetAtlas();
            sprite2.spriteName = "ToggleBase";
            sprite2.size = squareBrushCheckBox.size;
            sprite2.transform.parent = squareBrushCheckBox.transform;
            sprite2.transform.localPosition = Vector3.zero;
            squareBrushCheckBox.checkedBoxObject = sprite2.AddUIComponent<UISprite>();

            ((UISprite)squareBrushCheckBox.checkedBoxObject).atlas = UIUtilities.GetAtlas();
            ((UISprite)squareBrushCheckBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            squareBrushCheckBox.checkedBoxObject.size = squareBrushCheckBox.size;
            squareBrushCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            squareBrushCheckBox.isChecked = UserMod.Settings.SquareBrush;
            squareBrushCheckBox.eventCheckChanged += (c, e) =>
            {
                UserMod.Settings.SquareBrush = e;
                UserMod.Settings.Save();
            };
            squareBrushCheckBox.zOrder = 3;

            layoutPanel3 = AddUIComponent<UIPanel>();
            layoutPanel3.size = new Vector2(width, 16);
            layoutPanel3.autoLayout = true;
            layoutPanel3.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanel3.autoFitChildrenHorizontally = true;
            layoutPanel3.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanel3.zOrder = 3;

            overlayColorLabel = layoutPanel3.AddUIComponent<UILabel>();
            overlayColorLabel.text = OverlayColorLabelText;
            overlayColorLabel.textScale = Constants.UITextScale;
            overlayColorLabel.autoSize = true;
            overlayColorLabel.textAlignment = UIHorizontalAlignment.Left;
            overlayColorLabel.verticalAlignment = UIVerticalAlignment.Middle;
            overlayColorLabel.zOrder = 0;

            colorFieldTemplate = CreateColorField(layoutPanel3);
            colorFieldTemplate.size = Constants.UICheckboxSize;
            colorFieldTemplate.zOrder = 1;

            layoutPanel4 = AddUIComponent<UIPanel>();
            layoutPanel4.size = new Vector2(width, 1);
            layoutPanel4.zOrder = 4;
        }

        private UIColorField CreateColorField(UIComponent parent)
        {
            if (colorFieldTemplate == null)
            {
                UIComponent template = UITemplateManager.Get("LineTemplate");
                if (template == null) return null;

                colorFieldTemplate = template.Find<UIColorField>("LineColor");
                if (colorFieldTemplate == null) return null;
            }

            UIColorField cF = Instantiate(colorFieldTemplate.gameObject).GetComponent<UIColorField>();
            parent.AttachUIComponent(cF.gameObject);
            cF.name = "ForestBrushColorField";
            cF.pickerPosition = UIColorField.ColorPickerPosition.RightBelow;
            cF.eventSelectedColorChanged += EventSelectedColorChangedHandler;
            cF.selectedColor = UserMod.Settings.OverlayColor;
            return cF;
        }

        private void EventSelectedColorChangedHandler(UIComponent component, Color value)
        {
            UserMod.Settings.OverlayColor = value;
            UserMod.Settings.Save();
        }

        public string AutoDensityLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");
        public string SquareBrushLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SQUAREBRUSH");
        public string OverlayColorLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-OVERLAYCOLOR");
    }
}
