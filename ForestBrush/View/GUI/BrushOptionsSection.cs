using System;
using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushOptionsSection : UIPanel
    {
        UIPanel layoutPanelSize;
        UILabel sizeLabel;
        internal UISlider sizeSlider;

        UIPanel layoutPanelStrength;
        UILabel strengthLabel;
        internal UISlider strengthSlider;

        UIPanel layoutPanelDensity;
        UILabel densityLabel;
        internal UISlider densitySlider;

        UIPanel layoutPanelAutoDensityColor;
        UILabel autoDensityLabel;

        internal UICheckBox autoDensityCheckBox;
        UILabel overlayColorLabel;
        UIColorField colorFieldTemplate;

        UIPanel layoutPanelSquareBrush;
        UILabel squareBrushLabel;

        internal UICheckBox squareBrushCheckBox;

        public override void Start()
        {
            base.Start();

            Setup();
            SetupSizePanel();
            SetupStrengthPanel();
            SetupDensityPanel();
            SetupAutoDensityPanel();
            SetupSquareBrushPanel();

            if(!UserMod.Settings.BrushOptionsOpen) Hide();
        }

        public override void OnDestroy()
        {
            sizeSlider.eventMouseUp -= SizeSlider_eventMouseUp;
            sizeSlider.eventValueChanged -= SizeSlider_eventValueChanged;
            strengthSlider.eventValueChanged -= StrengthSlider_eventValueChanged;
            strengthSlider.eventMouseUp -= StrengthSlider_eventMouseUp;
            densitySlider.eventValueChanged -= DensitySlider_eventValueChanged;
            densitySlider.eventMouseUp -= DensitySlider_eventMouseUp;
            autoDensityCheckBox.eventCheckChanged -= SutoDensityCheckBox_eventCheckChanged;
            squareBrushCheckBox.eventCheckChanged -= SquareBrushCheckBox_eventCheckChanged;
            if (colorFieldTemplate != null)
            {
                colorFieldTemplate.eventSelectedColorChanged -= ColorField_eventSelectedColorChangedHandler;
                colorFieldTemplate.eventColorPickerOpen -= ColorField_eventColorPickerOpen;
                colorFieldTemplate.eventColorPickerClose -= ColorField_eventColorPickerClose;
            }
            base.OnDestroy();
        }

        private void Setup()
        {
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 10, 0);
            width = parent.width;
        }

        private void SetupSizePanel()
        {
            layoutPanelSize = AddUIComponent<UIPanel>();
            layoutPanelSize.size = new Vector2(width, 10);
            layoutPanelSize.autoLayout = true;
            layoutPanelSize.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelSize.autoFitChildrenHorizontally = true;
            layoutPanelSize.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
            layoutPanelSize.zOrder = 0;

            //size slider
            sizeLabel = layoutPanelSize.AddUIComponent<UILabel>();
            sizeLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SIZE");
            sizeLabel.textScale = Constants.UITextScale;
            sizeLabel.autoSize = false;
            sizeLabel.width = 80f;
            sizeLabel.disabledTextColor = new Color32(100, 100, 100, 255);
            sizeLabel.textAlignment = UIHorizontalAlignment.Left;
            sizeLabel.verticalAlignment = UIVerticalAlignment.Middle;
            sizeLabel.zOrder = 0;

            sizeSlider = layoutPanelSize.AddUIComponent<UISlider>();
            sizeSlider.atlas = ResourceLoader.Atlas;
            sizeSlider.size = new Vector2(400f - sizeLabel.width - 30f, 5f);
            sizeSlider.color = new Color32(0, 0, 0, 255);
            sizeSlider.disabledColor = new Color32(190, 190, 190, 255);
            sizeSlider.minValue = 1f;
            sizeSlider.maxValue = ForestBrush.Instance.Tool.Tweaker.MaxSize;
            sizeSlider.stepSize = 1f;
            sizeSlider.value = UserMod.Settings.SelectedBrush.Options.Size;
            sizeSlider.scrollWheelAmount = 1f;
            sizeSlider.eventValueChanged += SizeSlider_eventValueChanged;
            sizeSlider.eventMouseUp += SizeSlider_eventMouseUp;
            sizeSlider.backgroundSprite = ResourceLoader.WhiteRect;
            sizeSlider.tooltip = UserMod.Settings.SelectedBrush.Options.Size.ToString();
            sizeSlider.pivot = UIPivotPoint.TopLeft;
            sizeSlider.zOrder = 1;

            UISprite thumb = sizeSlider.AddUIComponent<UISprite>();
            thumb.atlas = ResourceLoader.Atlas;
            thumb.size = new Vector2(20, 20);
            thumb.spriteName = ResourceLoader.IconPolicyForest;
            sizeSlider.thumbObject = thumb;
        }

        private void SizeSlider_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            UserMod.SaveSettings();
        }

        private void SizeSlider_eventValueChanged(UIComponent component, float value)
        {
            UserMod.Settings.SelectedBrush.Options.Size = value;
            sizeSlider.tooltip = UserMod.Settings.SelectedBrush.Options.Size.ToString();
            sizeSlider.RefreshTooltip();
        }

        internal void LocaleChanged()
        {
            sizeLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SIZE");
            strengthLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-STRENGTH");
            densityLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY");
            autoDensityLabel.text = AutoDensityLabelText;
            squareBrushLabel.text = SquareBrushLabelText;
            if(overlayColorLabel != null) overlayColorLabel.text = OverlayColorLabelText;
        }

        private void SetupStrengthPanel()
        {
            layoutPanelStrength = AddUIComponent<UIPanel>();
            layoutPanelStrength.size = new Vector2(width, 10);
            layoutPanelStrength.autoLayout = true;
            layoutPanelStrength.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelStrength.autoFitChildrenHorizontally = true;
            layoutPanelStrength.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
            layoutPanelStrength.zOrder = 1;

            strengthLabel = layoutPanelStrength.AddUIComponent<UILabel>();
            strengthLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-STRENGTH");
            strengthLabel.textScale = Constants.UITextScale;
            strengthLabel.autoSize = false;
            strengthLabel.width = 80f;
            strengthLabel.disabledTextColor = new Color32(100, 100, 100, 255);
            strengthLabel.textAlignment = UIHorizontalAlignment.Left;
            strengthLabel.verticalAlignment = UIVerticalAlignment.Middle;
            strengthLabel.zOrder = 0;

            strengthSlider = layoutPanelStrength.AddUIComponent<UISlider>();
            strengthSlider.atlas = ResourceLoader.Atlas;
            strengthSlider.size = new Vector2(400f - strengthLabel.width - 30f, 5f);
            strengthSlider.color = new Color32(0, 0, 0, 255);
            strengthSlider.disabledColor = new Color32(190, 190, 190, 255);
            strengthSlider.minValue = 0.01f;
            strengthSlider.maxValue = 1f;
            strengthSlider.stepSize = 0.01f;
            strengthSlider.value = UserMod.Settings.SelectedBrush.Options.Strength;
            strengthSlider.scrollWheelAmount = 0.01f;
            strengthSlider.tooltip = Math.Round(strengthSlider.value * 100, 1, MidpointRounding.AwayFromZero) + "%";
            strengthSlider.eventValueChanged += StrengthSlider_eventValueChanged;
            strengthSlider.eventMouseUp += StrengthSlider_eventMouseUp;
            strengthSlider.backgroundSprite = ResourceLoader.WhiteRect;
            strengthSlider.zOrder = 1;
            strengthSlider.pivot = UIPivotPoint.TopLeft;

            UISprite thumb1 = strengthSlider.AddUIComponent<UISprite>();
            thumb1.atlas = ResourceLoader.Atlas;
            thumb1.size = new Vector2(20, 20);
            thumb1.spriteName = ResourceLoader.IconPolicyForest;
            strengthSlider.thumbObject = thumb1;
        }

        private void StrengthSlider_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            UserMod.SaveSettings();
        }

        private void StrengthSlider_eventValueChanged(UIComponent component, float value)
        {
            UserMod.Settings.SelectedBrush.Options.Strength = value;
            strengthSlider.tooltip = Math.Round(value * 100, 1) + "%";
            strengthSlider.RefreshTooltip();
        }

        private void SetupDensityPanel()
        {
            layoutPanelDensity = AddUIComponent<UIPanel>();
            layoutPanelDensity.size = new Vector2(width, 10);
            layoutPanelDensity.autoLayout = true;
            layoutPanelDensity.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelDensity.autoFitChildrenHorizontally = true;
            layoutPanelDensity.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
            layoutPanelDensity.zOrder = 2;

            densityLabel = layoutPanelDensity.AddUIComponent<UILabel>();
            densityLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-DENSITY");
            densityLabel.textScale = Constants.UITextScale;
            densityLabel.autoSize = false;
            densityLabel.width = 80f;
            densityLabel.disabledTextColor = new Color32(100, 100, 100, 255);
            densityLabel.textAlignment = UIHorizontalAlignment.Left;
            densityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            densityLabel.zOrder = 0;
            densityLabel.isEnabled = !UserMod.Settings.SelectedBrush.Options.AutoDensity;

            densitySlider = layoutPanelDensity.AddUIComponent<UISlider>();
            densitySlider.atlas = ResourceLoader.Atlas;
            densitySlider.size = new Vector2(400f - densityLabel.width - 30f, 5f);
            densitySlider.color = new Color32(0, 0, 0, 255);
            densitySlider.disabledColor = new Color32(75, 75, 75, 255);
            densitySlider.minValue = 0f;
            densitySlider.maxValue = 16f;
            densitySlider.stepSize = 0.1f;
            densitySlider.value = 16 - UserMod.Settings.SelectedBrush.Options.Density;
            densitySlider.scrollWheelAmount = 0.1f;
            densitySlider.eventValueChanged += DensitySlider_eventValueChanged;
            densitySlider.eventMouseUp += DensitySlider_eventMouseUp;
            densitySlider.backgroundSprite = ResourceLoader.WhiteRect;
            densitySlider.zOrder = 1;
            densitySlider.pivot = UIPivotPoint.TopLeft;
            densitySlider.arbitraryPivotOffset = new Vector2(0f, 3f);
            densitySlider.isEnabled = !UserMod.Settings.SelectedBrush.Options.AutoDensity;

            UISprite thumb1 = densitySlider.AddUIComponent<UISprite>();
            thumb1.atlas = ResourceLoader.Atlas;
            thumb1.size = new Vector2(20, 20);
            thumb1.spriteName = ResourceLoader.IconPolicyForest;
            thumb1.disabledColor = new Color32(100, 100, 100, 255);
            densitySlider.thumbObject = thumb1;
        }

        private void DensitySlider_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            UserMod.SaveSettings();
        }

        private void DensitySlider_eventValueChanged(UIComponent component, float value)
        {
            UserMod.Settings.SelectedBrush.Options.Density = 16f - value;
        }

        private void SetupAutoDensityPanel()
        {
            layoutPanelAutoDensityColor = AddUIComponent<UIPanel>();
            layoutPanelAutoDensityColor.size = new Vector2(width, 16);
            layoutPanelAutoDensityColor.autoLayout = true;
            layoutPanelAutoDensityColor.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelAutoDensityColor.autoFitChildrenHorizontally = true;
            layoutPanelAutoDensityColor.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
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
            sprite.atlas = ResourceLoader.Atlas;
            sprite.spriteName = ResourceLoader.CheckBoxSpriteUnchecked;
            sprite.size = autoDensityCheckBox.size;
            sprite.transform.parent = autoDensityCheckBox.transform;
            sprite.transform.localPosition = Vector3.zero;
            autoDensityCheckBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)autoDensityCheckBox.checkedBoxObject).atlas = ResourceLoader.Atlas;
            ((UISprite)autoDensityCheckBox.checkedBoxObject).spriteName = ResourceLoader.CheckBoxSpriteChecked;
            autoDensityCheckBox.checkedBoxObject.size = autoDensityCheckBox.size;
            autoDensityCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            autoDensityCheckBox.isChecked = UserMod.Settings.SelectedBrush.Options.AutoDensity;
            autoDensityCheckBox.eventCheckChanged += SutoDensityCheckBox_eventCheckChanged;
            autoDensityCheckBox.zOrder = 0;

            var colorField = CreateColorField(layoutPanelAutoDensityColor);
            if (colorField != null)
            {
                overlayColorLabel = layoutPanelAutoDensityColor.AddUIComponent<UILabel>();
                overlayColorLabel.text = OverlayColorLabelText;
                overlayColorLabel.textScale = Constants.UITextScale;
                overlayColorLabel.autoSize = true;
                overlayColorLabel.textAlignment = UIHorizontalAlignment.Left;
                overlayColorLabel.verticalAlignment = UIVerticalAlignment.Middle;
                overlayColorLabel.zOrder = 3;
                colorFieldTemplate = colorField;
                colorFieldTemplate.size = Constants.UIColorFieldSize;
                colorFieldTemplate.zOrder = 2;
            }
        }

        private void SutoDensityCheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            UserMod.Settings.SelectedBrush.Options.AutoDensity = value;
            densityLabel.isEnabled = densitySlider.isEnabled = !value;
            UserMod.SaveSettings();
        }

        private void SetupSquareBrushPanel()
        {
            layoutPanelSquareBrush = AddUIComponent<UIPanel>();
            layoutPanelSquareBrush.size = new Vector2(width, 16);
            layoutPanelSquareBrush.autoLayout = true;
            layoutPanelSquareBrush.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelSquareBrush.autoFitChildrenHorizontally = true;
            layoutPanelSquareBrush.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
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
            sprite2.atlas = ResourceLoader.Atlas;
            sprite2.spriteName = ResourceLoader.CheckBoxSpriteUnchecked;
            sprite2.size = squareBrushCheckBox.size;
            sprite2.transform.parent = squareBrushCheckBox.transform;
            sprite2.transform.localPosition = Vector3.zero;
            squareBrushCheckBox.checkedBoxObject = sprite2.AddUIComponent<UISprite>();

            ((UISprite)squareBrushCheckBox.checkedBoxObject).atlas = ResourceLoader.Atlas;
            ((UISprite)squareBrushCheckBox.checkedBoxObject).spriteName = ResourceLoader.CheckBoxSpriteChecked;
            squareBrushCheckBox.checkedBoxObject.size = squareBrushCheckBox.size;
            squareBrushCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            squareBrushCheckBox.isChecked = UserMod.Settings.SelectedBrush.Options.IsSquare;
            squareBrushCheckBox.eventCheckChanged += SquareBrushCheckBox_eventCheckChanged;
            squareBrushCheckBox.zOrder = 0;
        }

        private void SquareBrushCheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            UserMod.Settings.SelectedBrush.Options.IsSquare = value;
            UserMod.SaveSettings();
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

            UIColorField colorField = Instantiate(colorFieldTemplate.gameObject).GetComponent<UIColorField>();
            parent.AttachUIComponent(colorField.gameObject);
            colorField.name = "ForestBrushColorField";
            colorField.pickerPosition = UIColorField.ColorPickerPosition.RightBelow;
            colorField.eventSelectedColorChanged += ColorField_eventSelectedColorChangedHandler;
            colorField.eventColorPickerOpen += ColorField_eventColorPickerOpen;
            colorField.eventColorPickerClose += ColorField_eventColorPickerClose;
            colorField.selectedColor = UserMod.Settings.SelectedBrush.Options.OverlayColor;
            return colorField;
        }

        private void ColorField_eventColorPickerClose(UIColorField colorField, UIColorPicker popup, ref bool overridden)
        {
            colorField.triggerButton.isInteractive = true;
        }

        private void ColorField_eventColorPickerOpen(UIColorField colorField, UIColorPicker popup, ref bool overridden)
        {
            colorField.triggerButton.isInteractive = false;
        }

        private void ColorField_eventSelectedColorChangedHandler(UIComponent component, Color value)
        {
            UserMod.Settings.SelectedBrush.Options.OverlayColor = value;
            UserMod.SaveSettings();
        }

        public string AutoDensityLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");
        public string SquareBrushLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SQUAREBRUSH");
        public string OverlayColorLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-OVERLAYCOLOR");

        internal void LoadBrush(Brush brush)
        {
            sizeSlider.value = brush.Options.Size;
            sizeSlider.tooltip = brush.Options.Size.ToString();
            strengthSlider.value = brush.Options.Strength;
            strengthSlider.tooltip = Math.Round(brush.Options.Strength * 100, 1) + "%";
            densitySlider.value = 16f - brush.Options.Density;
            autoDensityCheckBox.isChecked = brush.Options.AutoDensity;
            if (colorFieldTemplate != null) colorFieldTemplate.selectedColor = brush.Options.OverlayColor;
            squareBrushCheckBox.isChecked = brush.Options.IsSquare;
        }
    }
}
