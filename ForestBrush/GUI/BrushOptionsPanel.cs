using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushOptionsPanel : UIPanel
    {
        UIDragHandle dragHandle;
        UILabel titleLabel;
        UIButton closeButton;
        UILabel sizeLabel;
        public UISlider sizeSlider;
        UILabel densityLabel;
        public UISlider densitySlider;
        UILabel autoDensityLabel;
        public UICheckBox autoDensityCheckBox;
        UILabel squareBrushLabel;
        public UICheckBox squareBrushCheckBox;
        UILabel overlayColorLabel;
        public UIColorField colorFieldTemplate;
        UIPanel layoutPanelTitle;
        UIPanel layoutPanelSize;
        UIPanel layoutPanelDensity;
        UIPanel layoutPanelAutoDensityColor;
        UIPanel layoutPanelSquareBrush;
        UIPanel layoutPanelSpace;

        public override void Start()
        {
            base.Start();

            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 0, 14);          


            width = 400f;
            relativePosition = new Vector3(0f, parent.height + 1f);
            atlas = UIUtilities.GetAtlas();
            backgroundSprite = "MenuPanel";
            isVisible = false;
            isInteractive = true;


            layoutPanelTitle = AddUIComponent<UIPanel>();
            layoutPanelTitle.size = new Vector2(width, 40);
            layoutPanelTitle.zOrder = 0;

            titleLabel = layoutPanelTitle.AddUIComponent<UILabel>();
            titleLabel.autoSize = titleLabel.autoHeight = false;
            titleLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-TITLE");
            titleLabel.textScale = Constants.UITextScale;
            titleLabel.verticalAlignment = UIVerticalAlignment.Middle;
            titleLabel.textAlignment = UIHorizontalAlignment.Center;
            titleLabel.size = new Vector2(layoutPanelTitle.width, 40f);
            titleLabel.relativePosition = Vector3.zero;

            dragHandle = layoutPanelTitle.AddUIComponent<UIDragHandle>();
            dragHandle.size = titleLabel.size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;

            closeButton = layoutPanelTitle.AddUIComponent<UIButton>();
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

            layoutPanelSize = AddUIComponent<UIPanel>();
            layoutPanelSize.size = new Vector2(width, 16);
            layoutPanelSize.autoLayout = true;
            layoutPanelSize.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelSize.autoFitChildrenHorizontally = true;
            layoutPanelSize.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanelSize.zOrder = 1;

            //size slider
            sizeLabel = layoutPanelSize.AddUIComponent<UILabel>();
            sizeLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SIZE");
            sizeLabel.textScale = Constants.UITextScale;
            sizeLabel.autoSize = true;
            sizeLabel.disabledTextColor = new Color32(100, 100, 100, 255);
            sizeLabel.textAlignment = UIHorizontalAlignment.Left;
            sizeLabel.verticalAlignment = UIVerticalAlignment.Middle;
            sizeLabel.zOrder = 0;

            sizeSlider = layoutPanelSize.AddUIComponent<UISlider>();
            sizeSlider.size = new Vector2(400f - sizeLabel.size.x - 42f, 13f);
            sizeSlider.color = new Color32(0, 0, 0, 255);
            sizeSlider.disabledColor = new Color32(190, 190, 190, 255);
            sizeSlider.minValue = 1f;
            sizeSlider.maxValue = 2000f;
            sizeSlider.stepSize = 1f;
            sizeSlider.value = CGSSerialized.BrushSize;
            sizeSlider.scrollWheelAmount = 1f;  
            sizeSlider.eventValueChanged += (c, e) =>
            {
                CGSSerialized.BrushSize.value = e;
                sizeSlider.tooltip = CGSSerialized.BrushSize.value.ToString();
                sizeSlider.RefreshTooltip();
            };
            sizeSlider.backgroundSprite = "OptionsScrollbarTrack";
            sizeSlider.tooltip = CGSSerialized.BrushSize.value.ToString();
            sizeSlider.zOrder = 1;

            UISprite thumb = sizeSlider.AddUIComponent<UISprite>();
            thumb.atlas = UIUtilities.GetAtlas();
            thumb.size = new Vector2(20, 20);
            thumb.spriteName = "IconPolicyForest";
            sizeSlider.thumbObject = thumb;


            //density slider
            layoutPanelDensity = AddUIComponent<UIPanel>();
            layoutPanelDensity.size = new Vector2(width, 16);
            layoutPanelDensity.autoLayout = true;
            layoutPanelDensity.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelDensity.autoFitChildrenHorizontally = true;
            layoutPanelDensity.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanelDensity.zOrder = 2;

            densityLabel = layoutPanelDensity.AddUIComponent<UILabel>();
            densityLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY");
            densityLabel.textScale = Constants.UITextScale;
            densityLabel.autoSize = true;
            densityLabel.disabledTextColor = new Color32(100, 100, 100, 255);
            densityLabel.textAlignment = UIHorizontalAlignment.Left;
            densityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            densityLabel.zOrder = 0;
            densityLabel.isEnabled = !CGSSerialized.AutoDensity;

            densitySlider = layoutPanelDensity.AddUIComponent<UISlider>();
            densitySlider.size = new Vector2(400f - densityLabel.size.x - 42f, 13f);
            densitySlider.color = new Color32(0, 0, 0, 255);
            densitySlider.disabledColor = new Color32(190, 190, 190, 255);
            densitySlider.minValue = 0f;
            densitySlider.maxValue = 16f;
            densitySlider.stepSize = 0.1f;
            densitySlider.value = 16 - CGSSerialized.BrushDensity;
            densitySlider.scrollWheelAmount = 0.1f;
            densitySlider.eventValueChanged += (c, e) =>
            {
                CGSSerialized.BrushDensity.value = 16f - e;
            };
            densitySlider.backgroundSprite = "OptionsScrollbarTrack";
            densitySlider.zOrder = 1;
            densitySlider.isEnabled = !CGSSerialized.AutoDensity;

            UISprite thumb1 = densitySlider.AddUIComponent<UISprite>();
            thumb1.atlas = UIUtilities.GetAtlas();
            thumb1.size = new Vector2(20, 20);
            thumb1.spriteName = "IconPolicyForest";
            densitySlider.thumbObject = thumb1;
            
            layoutPanelAutoDensityColor = AddUIComponent<UIPanel>();
            layoutPanelAutoDensityColor.size = new Vector2(width, 16);
            layoutPanelAutoDensityColor.autoLayout = true;
            layoutPanelAutoDensityColor.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelAutoDensityColor.autoFitChildrenHorizontally = true;
            layoutPanelAutoDensityColor.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanelAutoDensityColor.zOrder = 3;

            autoDensityLabel = layoutPanelAutoDensityColor.AddUIComponent<UILabel>();
            autoDensityLabel.text = AutoDensityLabelText;
            autoDensityLabel.textScale = Constants.UITextScale;
            autoDensityLabel.autoSize = true;
            autoDensityLabel.textAlignment = UIHorizontalAlignment.Left;
            autoDensityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            autoDensityLabel.zOrder = 1;

            autoDensityCheckBox = layoutPanelAutoDensityColor.AddUIComponent<UICheckBox>();
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
            autoDensityCheckBox.isChecked = CGSSerialized.AutoDensity;
            autoDensityCheckBox.eventCheckChanged += (c, e) =>
            {
                CGSSerialized.AutoDensity.value = e;
                densityLabel.isEnabled = densitySlider.isEnabled = !e;
            };
            autoDensityCheckBox.zOrder = 0;

            overlayColorLabel = layoutPanelAutoDensityColor.AddUIComponent<UILabel>();
            overlayColorLabel.text = OverlayColorLabelText;
            overlayColorLabel.textScale = Constants.UITextScale;
            overlayColorLabel.autoSize = true;
            overlayColorLabel.textAlignment = UIHorizontalAlignment.Left;
            overlayColorLabel.verticalAlignment = UIVerticalAlignment.Middle;
            overlayColorLabel.zOrder = 3;

            colorFieldTemplate = CreateColorField(layoutPanelAutoDensityColor);
            colorFieldTemplate.size = Constants.UICheckboxSize;
            colorFieldTemplate.zOrder = 2;

            layoutPanelSquareBrush = AddUIComponent<UIPanel>();
            layoutPanelSquareBrush.size = new Vector2(width, 16);
            layoutPanelSquareBrush.autoLayout = true;
            layoutPanelSquareBrush.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelSquareBrush.autoFitChildrenHorizontally = true;
            layoutPanelSquareBrush.autoLayoutPadding = new RectOffset(14, 0, 0, 0);
            layoutPanelSquareBrush.zOrder = 4;

            squareBrushLabel = layoutPanelSquareBrush.AddUIComponent<UILabel>();
            squareBrushLabel.text = SquareBrushLabelText;
            squareBrushLabel.textScale = Constants.UITextScale;
            squareBrushLabel.autoSize = true;
            squareBrushLabel.textAlignment = UIHorizontalAlignment.Left;
            squareBrushLabel.verticalAlignment = UIVerticalAlignment.Middle;
            squareBrushLabel.zOrder = 1;

            squareBrushCheckBox = layoutPanelSquareBrush.AddUIComponent<UICheckBox>();
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
            squareBrushCheckBox.isChecked = CGSSerialized.SquareBrush;
            squareBrushCheckBox.eventCheckChanged += (c, e) =>
            {
                CGSSerialized.SquareBrush.value = e;
            };
            squareBrushCheckBox.zOrder = 0;

            layoutPanelSpace = AddUIComponent<UIPanel>();
            layoutPanelSpace.size = new Vector2(width, 1);
            layoutPanelSpace.zOrder = 5;
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
            cF.selectedColor = UserMod.BrushSettings.OverlayColor;
            return cF;
        }

        private void EventSelectedColorChangedHandler(UIComponent component, Color value)
        {
            UserMod.BrushSettings.OverlayColor = value;
            UserMod.BrushSettings.Save();
        }

        public string AutoDensityLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");
        public string SquareBrushLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SQUAREBRUSH");
        public string OverlayColorLabelText => UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-OVERLAYCOLOR");
    }
}
