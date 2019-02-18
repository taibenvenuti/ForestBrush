using System;
using System.Linq;
using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class TreeItem : UIPanel, IUIFastListRow
    {
        public TreeInfo Prefab { get; private set; }
        UICheckBox includeCheckBox;
        UILabel treeNameLabel;
        UITextureSprite thumbNailSprite;
        UISlider probabilitySlider;
        UITextField probabilityTextField;
        bool initialized;

        private void Initialize(TreeInfo info)
        {
            //General
            name = info.GetUncheckedLocalizedTitle() + "ListItem";
            atlas = ResourceLoader.Atlas;
            width = parent.width;
            height = Constants.UIItemHeight;
            isVisible = true;
            isInteractive = true;

            //Thumbnail
            thumbNailSprite = AddUIComponent<UITextureSprite>();
            thumbNailSprite.texture = info.m_Atlas.sprites.Find(spr => spr.name == info.m_Thumbnail).texture;
            thumbNailSprite.size = new Vector2(50f, 54.5f);
            thumbNailSprite.relativePosition = new Vector3(Constants.UISpacing, (Constants.UIItemHeight - thumbNailSprite.height) / 2);

            //CheckBox
            includeCheckBox = AddUIComponent<UICheckBox>();
            includeCheckBox.size = Constants.UICheckboxSize + new Vector2(3f, 3f);
            var sprite = includeCheckBox.AddUIComponent<UISprite>();
            sprite.atlas =  ResourceLoader.Atlas;
            sprite.spriteName = ResourceLoader.CheckBoxSpriteUnchecked;
            sprite.size = includeCheckBox.size;
            sprite.relativePosition = Vector3.zero;
            includeCheckBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)includeCheckBox.checkedBoxObject).atlas =  ResourceLoader.Atlas;
            ((UISprite)includeCheckBox.checkedBoxObject).spriteName = ResourceLoader.CheckBoxSpriteChecked;
            includeCheckBox.checkedBoxObject.size = includeCheckBox.size;
            includeCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            includeCheckBox.eventCheckChanged += IncludeCheckBox_eventCheckChanged;
            includeCheckBox.relativePosition = new Vector3(70.0f, 15f);

            //Label
            treeNameLabel = AddUIComponent<UILabel>();
            treeNameLabel.text = info.GetUncheckedLocalizedTitle();
            treeNameLabel.autoSize = false;
            treeNameLabel.width = 255.0f;
            treeNameLabel.relativePosition = new Vector3(96.0f, 15.0f);

            //slider
            probabilitySlider = AddUIComponent<UISlider>();
            probabilitySlider.atlas = ResourceLoader.Atlas;
            probabilitySlider.size = new Vector2(230f, 5f);
            probabilitySlider.color = new Color32(55, 55, 55, 255);
            probabilitySlider.disabledColor = new Color32(100, 100, 100, 255);
            probabilitySlider.minValue = 1f;
            probabilitySlider.maxValue = 100f;
            probabilitySlider.stepSize = 1f;
            probabilitySlider.scrollWheelAmount = 1f;
            probabilitySlider.eventValueChanged += ProbabilitySlider_eventValueChanged;
            probabilitySlider.eventMouseUp += ProbabilitySlider_eventMouseUp;
            probabilitySlider.backgroundSprite = ResourceLoader.WhiteRect;
            probabilitySlider.pivot = UIPivotPoint.TopLeft;
            probabilitySlider.relativePosition = new Vector3(thumbNailSprite.width + Constants.UISpacing * 2, 42.0f);
            UISprite thumb = probabilitySlider.AddUIComponent<UISprite>();
            thumb.atlas = ResourceLoader.Atlas;
            thumb.size = new Vector2(8.0f, 15.0f);
            thumb.spriteName = ResourceLoader.TextFieldPanel;
            thumb.disabledColor = new Color32(140, 140, 140, 255);
            probabilitySlider.thumbObject = thumb;
            probabilitySlider.isEnabled = includeCheckBox.isChecked;

            //Textfield

            probabilityTextField = AddUIComponent<UITextField>();
            probabilityTextField.atlas = ResourceLoader.Atlas;
            probabilityTextField.size = new Vector2(35.0f, 19.0f);
            probabilityTextField.padding = new RectOffset(2, 2, 4, 0);
            probabilityTextField.builtinKeyNavigation = true;
            probabilityTextField.isInteractive = true;
            probabilityTextField.readOnly = false;
            probabilityTextField.horizontalAlignment = UIHorizontalAlignment.Center;
            probabilityTextField.selectionSprite = ResourceLoader.EmptySprite;
            probabilityTextField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            probabilityTextField.normalBgSprite = ResourceLoader.TextFieldPanelHovered;
            probabilityTextField.disabledBgSprite = ResourceLoader.TextFieldPanelHovered;
            probabilityTextField.textColor = new Color32(0, 0, 0, 255);
            probabilityTextField.textScale = 0.85f;
            probabilityTextField.disabledTextColor = new Color32(128, 128, 128, 255);
            probabilityTextField.color = new Color32(255, 255, 255, 255);
            probabilityTextField.disabledColor = new Color32(128, 128, 128, 255);
            probabilityTextField.relativePosition = new Vector3(318.0f, 34.0f);
            probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;
            probabilityTextField.eventKeyPress += ProbabilityTextField_eventKeyPress;
            probabilityTextField.eventLostFocus += ProbabilityTextField_eventLostFocus;
            probabilityTextField.eventGotFocus += ProbabilityTextField_eventGotFocus;
            probabilityTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-PROBABILITY");
            probabilityTextField.isEnabled = includeCheckBox.isChecked;

            probabilitySlider.value = GetProbability(info);
            includeCheckBox.isChecked = ForestBrush.Instance.Container.m_variations.Any(v => v.m_finalTree == info);
            probabilityTextField.text = probabilityTextField.isEnabled ? GetProbability(info).ToString() : 0.ToString();

            initialized = true;
        }

        private void ProbabilityTextField_eventGotFocus(UIComponent component, UIFocusEventParameter eventParam)
        {
            probabilityTextField.SelectAll();
        }

        internal void LocaleChanged()
        {
            probabilityTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-PROBABILITY");
        }

        private void ProbabilityTextField_eventLostFocus(UIComponent component, UIFocusEventParameter eventParam)
        {
            if (float.TryParse(probabilityTextField.text, out float value))
            {
                probabilitySlider.eventValueChanged -= ProbabilitySlider_eventValueChanged;
                probabilitySlider.value = value;
                probabilitySlider.eventValueChanged += ProbabilitySlider_eventValueChanged;
                SetProbability(value, Prefab);
                UserMod.SaveSettings();
            }
        }

        private void ProbabilityTextField_eventKeyPress(UIComponent component, UIKeyEventParameter eventParam)
        {
            char ch = eventParam.character;
            if (!char.IsControl(ch) && !char.IsDigit(ch))
            {
                eventParam.Use();
            }
            if(float.TryParse(probabilityTextField.text, out float result) && result > 100)
            {
                probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
                probabilityTextField.text = 100.ToString();
                probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;
            }
            if (eventParam.keycode == KeyCode.Escape)
            {
                probabilityTextField.Unfocus();
            }
        }

        private void ProbabilityTextField_eventTextChanged(UIComponent component, string value)
        {
            if (float.TryParse(value, out float probability))
            {
                if (probability > 100.0f)
                {
                    probability = 100.0f;
                    probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
                    probabilityTextField.text = probability.ToString();
                    probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;
                }
            }
        }

        private void ProbabilitySlider_eventValueChanged(UIComponent component, float value)
        {
            probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
            probabilityTextField.text = value.ToString();
            probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;

        }

        private void ProbabilitySlider_eventMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            SetProbability(probabilitySlider.value, Prefab);
            UserMod.SaveSettings();
        }

        private void IncludeCheckBox_eventCheckChanged(UIComponent component, bool value)
        {
            if (ForestBrush.Instance?.BrushTool?.Brush?.Trees != null && ForestBrush.Instance.BrushTool.Brush.Trees.Count > 100 && value)
            {
                ToggleCheckbox(false);
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                 Translation.Instance.GetTranslation("FOREST-BRUSH-MODAL-LIMITREACHED-TITLE"),
                 Translation.Instance.GetTranslation("FOREST-BRUSH-MODAL-LIMITREACHED-MESSAGE-ONE"),
                 false);
                return;
            }
            RefreshProbabilityUI(value);
            bool updateAll = false;
            if (includeCheckBox.hasFocus && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand)))
                updateAll = true;
            ForestBrush.Instance.BrushTool.UpdateTreeList(Prefab, value, updateAll);
        }

        private void RefreshProbabilityUI(bool value)
        {
            probabilitySlider.isEnabled = probabilityTextField.isEnabled = value;
            if (value)
            {
                probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
                probabilityTextField.text = GetProbability(Prefab).ToString();
                probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;
            }
            else
            {
                probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
                probabilityTextField.text = 0.ToString();
                probabilityTextField.eventTextChanged += ProbabilityTextField_eventTextChanged;
            }
        }

        private void SetProbability(float probability, TreeInfo info)
        {
            Tree tree = ForestBrush.Instance.BrushTool.Brush.Trees.Find(t => t.Name == info.name);
            if (tree != null) tree.Probability = probability;
            ForestBrush.Instance.BrushTool.UpdateBrushPrefabProbabilities();
        }

        private float GetProbability(TreeInfo info)
        {
            Tree tree = ForestBrush.Instance.BrushTool.Brush.Trees.Find(t => t.Name == info.name);
            return tree != null ? tree.Probability : 100.0f;
        }

        public override void OnDestroy()
        {
            includeCheckBox.eventCheckChanged -= IncludeCheckBox_eventCheckChanged;
            probabilitySlider.eventValueChanged -= ProbabilitySlider_eventValueChanged;
            probabilitySlider.eventMouseUp -= ProbabilitySlider_eventMouseUp;
            probabilityTextField.eventTextChanged -= ProbabilityTextField_eventTextChanged;
            probabilityTextField.eventKeyPress -= ProbabilityTextField_eventKeyPress;
            probabilityTextField.eventLostFocus -= ProbabilityTextField_eventLostFocus;
            probabilityTextField.eventGotFocus -= ProbabilityTextField_eventGotFocus;
            base.OnDestroy();
        }

        public void ToggleCheckbox(bool value)
        {
            RefreshProbabilityUI(value);
            includeCheckBox.eventCheckChanged -= IncludeCheckBox_eventCheckChanged;
            includeCheckBox.isChecked = value;
            includeCheckBox.eventCheckChanged += IncludeCheckBox_eventCheckChanged;
        }

        public void UpdateCheckbox()
        {
            includeCheckBox.isChecked = ForestBrush.Instance.Container.m_variations.Any(v => v.m_finalTree == Prefab);
        }

        public void Deselect(bool isRowOdd)
        {
            // Needed for interface
        }

        public void Display(object data, bool isRowOdd)
        {
            if (data == null) return;
            try
            {
                Prefab = data as TreeInfo;
                if(!initialized) Initialize(Prefab);
                includeCheckBox.isChecked = ForestBrush.Instance.Container.m_variations.Any(v => v.m_finalTree == Prefab);
                treeNameLabel.text = Prefab.GetUncheckedLocalizedTitle();
                thumbNailSprite.texture = Prefab.m_Atlas.sprites.Find(spr => spr.name == Prefab.m_Thumbnail).texture;
                probabilityTextField.isEnabled = includeCheckBox.isChecked;
                probabilityTextField.text = probabilityTextField.isEnabled ? GetProbability(Prefab).ToString() : 0.ToString();
                probabilitySlider.isEnabled = includeCheckBox.isChecked;
                probabilitySlider.value = GetProbability(Prefab);
                backgroundSprite = "";
                if (isRowOdd)
                {
                    backgroundSprite = ResourceLoader.ListItemHover;
                    probabilitySlider.disabledColor = new Color32(110, 135, 135, 255);
                    probabilitySlider.thumbObject.disabledColor = new Color32(190, 210, 210, 255);
                    probabilityTextField.disabledTextColor = new Color32(110, 135, 135, 255); ;
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning(exception + "Stacktrace: " + exception.StackTrace, this);
                
            }
        }

        public void Select(bool isRowOdd)
        {
            // Needed of interface
        }
    }
}
