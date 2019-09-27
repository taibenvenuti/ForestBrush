using System;
using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushOptionsSection : UIPanel
    {
        ForestBrushPanel owner;

        UIPanel layoutPanelSize;
        UILabel sizeLabel;
        internal UISlider sizeSlider;

        UIPanel layoutPanelStrength;
        UILabel strengthLabel;
        internal UISlider strengthSlider;

        UIPanel layoutPanelDensity;
        UILabel densityLabel;
        internal UISlider densitySlider;

        UIPanel layoutPanelAutoDensityReset;
        UIPanel layoutPanelAutoDensity;
        UIPanel layoutPanelReset;
        UILabel autoDensityLabel;
        internal UICheckBox autoDensityCheckBox;
        UIButton resetButton;


        public override void Start()
        {
            base.Start();

            Setup();
            SetupSizePanel();
            SetupStrengthPanel();
            SetupDensityPanel();
            SetupAutoDensityPanel();

            LoadBrush(UserMod.Settings.SelectedBrush);

            if (!UserMod.Settings.BrushOptionsOpen)
            {
                owner.BrushSelectSection.UnfocusHideOptionsSectionButton();
                Hide();
            }
        }

        public override void OnDestroy()
        {
            sizeSlider.eventMouseUp -= SizeSlider_eventMouseUp;
            sizeSlider.eventValueChanged -= SizeSlider_eventValueChanged;
            strengthSlider.eventValueChanged -= StrengthSlider_eventValueChanged;
            strengthSlider.eventMouseUp -= StrengthSlider_eventMouseUp;
            densitySlider.eventValueChanged -= DensitySlider_eventValueChanged;
            densitySlider.eventMouseUp -= DensitySlider_eventMouseUp;
            autoDensityCheckBox.eventCheckChanged -= AutoDensityCheckBox_eventCheckChanged;
            base.OnDestroy();
        }

        private void Setup()
        {
            owner = (ForestBrushPanel)parent;
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
            densitySlider.minValue = 0.0f;
            densitySlider.maxValue = 15.5f;
            densitySlider.stepSize = 0.155f;
            densitySlider.value = 16.0f - UserMod.Settings.SelectedBrush.Options.Density;
            densitySlider.scrollWheelAmount = 0.155f;
            densitySlider.tooltip = Math.Round(densitySlider.value * 6.451612903225806, 1, MidpointRounding.AwayFromZero) + "%";
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

        internal void UpdateBindings(Brush.BrushOptions options)
        {
            sizeSlider.value = options.Size;
            strengthSlider.value = options.Strength;
            densitySlider.value = 16f - options.Density;
        }

        private void DensitySlider_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            UserMod.SaveSettings();
        }

        private void DensitySlider_eventValueChanged(UIComponent component, float value)
        {
            densitySlider.tooltip = Math.Round(densitySlider.value * 6.451612903225806, 1, MidpointRounding.AwayFromZero) + "%";
            densitySlider.RefreshTooltip();
            UserMod.Settings.SelectedBrush.Options.Density = 16f - value;
        }

        private void SetupAutoDensityPanel()
        {
            layoutPanelAutoDensityReset = AddUIComponent<UIPanel>();
            layoutPanelAutoDensityReset.size = new Vector2(width, 16);
            layoutPanelAutoDensityReset.autoLayout = true;
            layoutPanelAutoDensityReset.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelAutoDensityReset.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            layoutPanelAutoDensityReset.zOrder = 3;

            layoutPanelAutoDensity = layoutPanelAutoDensityReset.AddUIComponent<UIPanel>();
            layoutPanelAutoDensity.size = new Vector2(width / 2, 16);
            layoutPanelAutoDensity.autoLayout = true;
            layoutPanelAutoDensity.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelAutoDensity.autoLayoutPadding = new RectOffset(10, 0, 0, 0);
            layoutPanelAutoDensity.zOrder = 0;

            autoDensityCheckBox = layoutPanelAutoDensity.AddUIComponent<UICheckBox>();
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
            autoDensityCheckBox.eventCheckChanged += AutoDensityCheckBox_eventCheckChanged;
            autoDensityCheckBox.zOrder = 0;

            autoDensityLabel = layoutPanelAutoDensity.AddUIComponent<UILabel>();
            autoDensityLabel.text = AutoDensityLabelText;
            autoDensityLabel.textScale = Constants.UITextScale;
            autoDensityLabel.padding = new RectOffset(0, 0, 3, 0);
            autoDensityLabel.autoSize = true;
            autoDensityLabel.textAlignment = UIHorizontalAlignment.Left;
            autoDensityLabel.verticalAlignment = UIVerticalAlignment.Middle;
            autoDensityLabel.zOrder = 1;

            layoutPanelReset = layoutPanelAutoDensityReset.AddUIComponent<UIPanel>();
            layoutPanelReset.size = new Vector2(width / 2, 16);
            layoutPanelReset.autoLayout = true;
            layoutPanelReset.autoLayoutDirection = LayoutDirection.Horizontal;
            layoutPanelReset.autoLayoutStart = LayoutStart.TopRight;
            layoutPanelReset.autoLayoutPadding = new RectOffset(0, 10, 0, 0);
            layoutPanelReset.zOrder = 1;

            resetButton = layoutPanelReset.AddUIComponent<UIButton>();
            resetButton.atlas = ResourceLoader.Atlas;
            resetButton.textScale = Constants.UITextScale;
            resetButton.textPadding = new RectOffset(4, 4, 4, 2);
            resetButton.pivot = UIPivotPoint.TopRight;
            resetButton.anchor = UIAnchorStyle.Right;
            resetButton.zOrder = 0;
            resetButton.normalBgSprite = "ButtonSmall";
            resetButton.hoveredBgSprite = "ButtonSmallHovered";
            resetButton.pressedBgSprite = "ButtonSmallPressed";
            resetButton.focusedBgSprite = "ButtonSmall";
            resetButton.text = Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-RESET");
            resetButton.eventClicked += ResetButton_eventClicked;
            resetButton.autoSize = true;
        }

        private void ResetButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            Brush.BrushOptions options = Brush.BrushOptions.Default();
            UserMod.Settings.SelectedBrush.Options = options;
            UpdateBindings(options);
            autoDensityCheckBox.isChecked = options.AutoDensity;
            UserMod.SaveSettings();
        }

        private void AutoDensityCheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            UserMod.Settings.SelectedBrush.Options.AutoDensity = value;
            densityLabel.isEnabled = densitySlider.isEnabled = !value;
            UserMod.SaveSettings();
        }

        public string AutoDensityLabelText => Translation.Instance.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-AUTODENSITY");

        internal void LoadBrush(Brush brush)
        {
            UpdateBindings(brush.Options);
            autoDensityCheckBox.isChecked = brush.Options.AutoDensity;
        }
    }
}
