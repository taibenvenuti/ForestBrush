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
            Hide();
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
            sizeSlider.maxValue = ForestBrushMod.Instance.BrushTweaker.MaxSize;
            sizeSlider.stepSize = 1f;
            sizeSlider.value = UserMod.Settings.SelectedBrush.Options.Size;
            sizeSlider.scrollWheelAmount = 1f;
            sizeSlider.eventValueChanged += (c, e) =>
            {
                UserMod.Settings.SelectedBrush.Options.Size = e;
                sizeSlider.tooltip = UserMod.Settings.SelectedBrush.Options.Size.ToString();
                sizeSlider.RefreshTooltip();
            };
            sizeSlider.eventMouseUp += (c, e) => UserMod.SaveSettings();
            sizeSlider.backgroundSprite = ResourceLoader.OptionsScrollbarTrack;
            sizeSlider.tooltip = UserMod.Settings.SelectedBrush.Options.Size.ToString();
            sizeSlider.pivot = UIPivotPoint.TopLeft;
            sizeSlider.zOrder = 1;

            UISprite thumb = sizeSlider.AddUIComponent<UISprite>();
            thumb.atlas = ResourceLoader.Atlas;
            thumb.size = new Vector2(20, 20);
            thumb.spriteName = ResourceLoader.IconPolicyForest;
            sizeSlider.thumbObject = thumb;
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
            strengthSlider.eventValueChanged += (c, p) =>
            {
                UserMod.Settings.SelectedBrush.Options.Strength = p;
                strengthSlider.tooltip = Math.Round(p * 100, 1) + "%";
                strengthSlider.RefreshTooltip();
            };
            strengthSlider.eventMouseUp += (c, e) => UserMod.SaveSettings();
            strengthSlider.backgroundSprite = ResourceLoader.OptionsScrollbarTrack;
            strengthSlider.zOrder = 1;
            strengthSlider.pivot = UIPivotPoint.TopLeft;

            UISprite thumb1 = strengthSlider.AddUIComponent<UISprite>();
            thumb1.atlas = ResourceLoader.Atlas;
            thumb1.size = new Vector2(20, 20);
            thumb1.spriteName = ResourceLoader.IconPolicyForest;
            strengthSlider.thumbObject = thumb1;
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
            densitySlider.disabledColor = new Color32(190, 190, 190, 255);
            densitySlider.minValue = 0f;
            densitySlider.maxValue = 16f;
            densitySlider.stepSize = 0.1f;
            densitySlider.value = 16 - UserMod.Settings.SelectedBrush.Options.Density;
            densitySlider.scrollWheelAmount = 0.1f;
            densitySlider.eventValueChanged += (c, e) =>
            {
                UserMod.Settings.SelectedBrush.Options.Density = 16f - e;
            };
            densitySlider.eventMouseUp += (c, e) => UserMod.SaveSettings();
            densitySlider.backgroundSprite = ResourceLoader.OptionsScrollbarTrack;
            densitySlider.zOrder = 1;
            densitySlider.pivot = UIPivotPoint.TopLeft;
            densitySlider.arbitraryPivotOffset = new Vector2(0f, 3f);
            densitySlider.isEnabled = !UserMod.Settings.SelectedBrush.Options.AutoDensity;

            UISprite thumb1 = densitySlider.AddUIComponent<UISprite>();
            thumb1.atlas = ResourceLoader.Atlas;
            thumb1.size = new Vector2(20, 20);
            thumb1.spriteName = ResourceLoader.IconPolicyForest;
            densitySlider.thumbObject = thumb1;
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
            autoDensityCheckBox.eventCheckChanged += (c, e) =>
            {
                UserMod.Settings.SelectedBrush.Options.AutoDensity = e;
                densityLabel.isEnabled = densitySlider.isEnabled = !e;
                UserMod.SaveSettings();
            };
            autoDensityCheckBox.zOrder = 0;

            if (UserMod.IsGame)
            {
                overlayColorLabel = layoutPanelAutoDensityColor.AddUIComponent<UILabel>();
                overlayColorLabel.text = OverlayColorLabelText;
                overlayColorLabel.textScale = Constants.UITextScale;
                overlayColorLabel.autoSize = true;
                overlayColorLabel.textAlignment = UIHorizontalAlignment.Left;
                overlayColorLabel.verticalAlignment = UIVerticalAlignment.Middle;
                overlayColorLabel.zOrder = 3;

                colorFieldTemplate = CreateColorField(layoutPanelAutoDensityColor);
                colorFieldTemplate.size = Constants.UIColorFieldSize;
                colorFieldTemplate.zOrder = 2;
            }
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
            squareBrushCheckBox.eventCheckChanged += (c, e) =>
            {
                UserMod.Settings.SelectedBrush.Options.IsSquare = e;
                UserMod.SaveSettings();
            };
            squareBrushCheckBox.zOrder = 0;
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
            cF.selectedColor = UserMod.Settings.SelectedBrush.Options.OverlayColor;
            return cF;
        }

        private void EventSelectedColorChangedHandler(UIComponent component, Color value)
        {
            UserMod.Settings.SelectedBrush.Options.OverlayColor = value;
            UserMod.SaveSettings();
        }

        public string AutoDensityLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");
        public string SquareBrushLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-SQUAREBRUSH");
        public string OverlayColorLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-OVERLAYCOLOR");

        internal void LoadBrush(ForestBrush brush)
        {
            sizeSlider.value = brush.Options.Size;
            sizeSlider.tooltip = brush.Options.Size.ToString();
            strengthSlider.value = brush.Options.Strength;
            strengthSlider.tooltip = Math.Round(brush.Options.Strength * 100, 1) + "%";
            densitySlider.value = brush.Options.Density;
            autoDensityCheckBox.isChecked = brush.Options.AutoDensity;
            if (UserMod.IsGame) colorFieldTemplate.selectedColor = brush.Options.OverlayColor;
            squareBrushCheckBox.isChecked = brush.Options.IsSquare;
        }
    }
}
