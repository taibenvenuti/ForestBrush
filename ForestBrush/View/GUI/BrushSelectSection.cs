using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using System;
using System.Linq;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushSelectSection : UIPanel
    {
        ForestBrushPanel owner;
        internal UIDropDown SelectBrushDropDown;
        UIButton toggleEditButton;
        UIButton toggleOptionsButton;
        UIButton selectBitmapButton;

        public override void Start()
        {
            base.Start();
            owner = (ForestBrushPanel)parent;
            height = 30f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoFitChildrenHorizontally = true;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(10, 0, 0, 0);

            SetupDropDown();
            SetupButtons();

            LoadBrush(UserMod.Settings.SelectedBrush);
        }

        public override void OnDestroy()
        {
            SelectBrushDropDown.eventSelectedIndexChanged -= SelectBrushDropDown_eventSelectedIndexChanged;
            SelectBrushDropDown.eventDropdownOpen -= SelectBrushDropDown_eventDropdownOpen;
            SelectBrushDropDown.eventDropdownClose -= SelectBrushDropDown_eventDropdownClose;
            toggleEditButton.eventClicked -= ToggleEditButton_eventClicked;
            toggleOptionsButton.eventClicked -= ToggleOptionsButton_eventClicked;
            selectBitmapButton.eventClicked -= SelectBitmapButton_eventClicked;
            base.OnDestroy();
        }
        private void SetupDropDown()
        {
            SelectBrushDropDown = AddUIComponent<UIDropDown>();
            SelectBrushDropDown.zOrder = 0;
            SelectBrushDropDown.atlas = ResourceLoader.Atlas;
            SelectBrushDropDown.size = new Vector2(260f, 30f);
            SelectBrushDropDown.items = GetDropdownItems();
            SelectBrushDropDown.listBackground = ResourceLoader.StylesDropboxListbox;
            SelectBrushDropDown.itemHeight = (int)Constants.UIButtonHeight;
            SelectBrushDropDown.itemHover = ResourceLoader.ListItemHover;
            SelectBrushDropDown.itemHighlight = ResourceLoader.ListItemHighlight;
            SelectBrushDropDown.normalBgSprite = ResourceLoader.CMStylesDropbox;
            SelectBrushDropDown.hoveredBgSprite = ResourceLoader.CMStylesDropboxHovered;
            SelectBrushDropDown.disabledBgSprite = "";
            SelectBrushDropDown.focusedBgSprite = "";
            SelectBrushDropDown.listWidth = 300;
            SelectBrushDropDown.listHeight = 500;
            SelectBrushDropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            SelectBrushDropDown.popupColor = Color.white;
            SelectBrushDropDown.popupTextColor = new Color32(170, 170, 170, 255);
            SelectBrushDropDown.zOrder = 1;
            SelectBrushDropDown.textScale = Constants.UITitleTextScale;
            SelectBrushDropDown.verticalAlignment = UIVerticalAlignment.Middle;
            SelectBrushDropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            SelectBrushDropDown.selectedIndex = GetDropdownItemsSelectedIndex();
            SelectBrushDropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            SelectBrushDropDown.itemPadding = new RectOffset(10, 0, 8, 0);
            SelectBrushDropDown.triggerButton = SelectBrushDropDown;
            SelectBrushDropDown.eventSelectedIndexChanged += SelectBrushDropDown_eventSelectedIndexChanged;
            SelectBrushDropDown.eventDropdownOpen += SelectBrushDropDown_eventDropdownOpen;
            SelectBrushDropDown.eventDropdownClose += SelectBrushDropDown_eventDropdownClose;
            SelectBrushDropDown.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-SELECT");
        }

        private void SelectBrushDropDown_eventDropdownOpen(UIDropDown dropdown, UIListBox popup, ref bool overridden)
        {
            SelectBrushDropDown.triggerButton.isInteractive = false;
        }

        private void SelectBrushDropDown_eventDropdownClose(UIDropDown dropdown, UIListBox popup, ref bool overridden)
        {
            SelectBrushDropDown.triggerButton.isInteractive = true;
        }

        internal void LocaleChanged()
        {
            SelectBrushDropDown.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-SELECT");
            toggleEditButton.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-EDIT");
            toggleOptionsButton.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-OPTIONS");
        }

        internal void LoadBrush(Brush brush)
        {
            UpdateDropDown();
        }

        internal void UnfocusEditSectionButton()
        {
            toggleEditButton.normalBgSprite = toggleEditButton.focusedBgSprite = ResourceLoader.SettingsDropbox;
        }

        internal void UnfocusShapesSectionButton()
        {
            selectBitmapButton.normalBgSprite = selectBitmapButton.focusedBgSprite = ResourceLoader.PaintBrushNormal;
        }

        internal void UnfocusHideOptionsSectionButton()
        {
            toggleOptionsButton.normalBgSprite = toggleOptionsButton.focusedBgSprite = ResourceLoader.OptionsDropbox;
        }

        private void SetupButtons()
        {
            toggleEditButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-EDIT"));
            toggleEditButton.zOrder = 1;
            toggleEditButton.atlas = ResourceLoader.ForestBrushAtlas;
            toggleEditButton.normalBgSprite = ResourceLoader.SettingsDropboxFocused;
            toggleEditButton.hoveredBgSprite = ResourceLoader.SettingsDropboxHovered;
            toggleEditButton.focusedBgSprite = ResourceLoader.SettingsDropboxFocused;
            toggleEditButton.pressedBgSprite = ResourceLoader.SettingsDropboxPressed;
            toggleEditButton.eventClicked += ToggleEditButton_eventClicked;

            toggleOptionsButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-OPTIONS"));
            toggleOptionsButton.zOrder = 2;
            toggleOptionsButton.atlas = ResourceLoader.ForestBrushAtlas;
            toggleOptionsButton.normalBgSprite = ResourceLoader.OptionsDropboxFocused;
            toggleOptionsButton.hoveredBgSprite = ResourceLoader.OptionsDropboxHovered;
            toggleOptionsButton.focusedBgSprite = ResourceLoader.OptionsDropboxFocused;
            toggleOptionsButton.pressedBgSprite = ResourceLoader.OptionsDropboxPressed;
            toggleOptionsButton.eventClicked += ToggleOptionsButton_eventClicked;

            selectBitmapButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-SELECTBRUSHSHAPE"));
            selectBitmapButton.size = new Vector2(30.0f, 30.0f);
            selectBitmapButton.relativePosition = new Vector2(10.0f, 0.0f);
            selectBitmapButton.zOrder = 3;
            selectBitmapButton.eventClicked += SelectBitmapButton_eventClicked;
            selectBitmapButton.atlas = ResourceLoader.ForestBrushAtlas;
            selectBitmapButton.normalBgSprite = ResourceLoader.PaintBrushFocused;
            selectBitmapButton.hoveredBgSprite = ResourceLoader.PaintBrushHovered;
            selectBitmapButton.focusedBgSprite = ResourceLoader.PaintBrushFocused;
            selectBitmapButton.pressedBgSprite = ResourceLoader.PaintBrushPressed;
        }

        private void SelectBitmapButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool shapeSelectorVisible = owner.ToggleBrushShapes();
            selectBitmapButton.normalBgSprite = selectBitmapButton.focusedBgSprite = shapeSelectorVisible ? ResourceLoader.PaintBrushFocused : ResourceLoader.PaintBrushNormal;
            if (shapeSelectorVisible) owner.ClampToScreen();
            UserMod.Settings.BrushShapesOpen = shapeSelectorVisible;
            UserMod.SaveSettings();
        }

        private void ToggleEditButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool editVisible = owner.ToggleBrushEdit();
            toggleEditButton.normalBgSprite = toggleEditButton.focusedBgSprite = editVisible ? ResourceLoader.SettingsDropboxFocused : ResourceLoader.SettingsDropbox;
            if (editVisible) owner.ClampToScreen();
            UserMod.Settings.BrushEditOpen = editVisible;
            UserMod.SaveSettings();
        }

        private void ToggleOptionsButton_eventClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool optionsVisible = owner.ToggleBrushOptions();
            toggleOptionsButton.normalBgSprite = toggleOptionsButton.focusedBgSprite = optionsVisible ? ResourceLoader.OptionsDropboxFocused : ResourceLoader.OptionsDropbox;
            if (optionsVisible) owner.ClampToScreen();
            UserMod.Settings.BrushOptionsOpen = optionsVisible;
            UserMod.SaveSettings();
        }

        internal void UpdateDropDown()
        {
            SelectBrushDropDown.items = GetDropdownItems();
            SelectBrushDropDown.eventSelectedIndexChanged -= SelectBrushDropDown_eventSelectedIndexChanged;
            SelectBrushDropDown.selectedIndex = GetDropdownItemsSelectedIndex();
            SelectBrushDropDown.eventSelectedIndexChanged += SelectBrushDropDown_eventSelectedIndexChanged;
        }

        private void SelectBrushDropDown_eventSelectedIndexChanged(UIComponent component, int index)
        {
            var brushName = SelectBrushDropDown.items[index];
            ForestBrush.Instance.BrushTool.UpdateTool(brushName);
            owner.BrushEditSection.ResetRenameError();
        }

        private string[] GetDropdownItems()
        {
            return UserMod.Settings.Brushes.Select(b => b.Name).OrderBy(x => x).ToArray();
        }

        private int GetDropdownItemsSelectedIndex()
        {
            return Array.IndexOf(SelectBrushDropDown.items, UserMod.Settings.SelectedBrush.Name);
        }
    }
}
