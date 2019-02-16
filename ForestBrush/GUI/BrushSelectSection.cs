using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using System;
using System.Linq;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushSelectSection: UIPanel
    {
        ForestBrushPanel father;
        internal UIDropDown SelectBrushDropDown;
        UIButton toggleEditButton;
        UIButton toggleOptionsButton;

        public override void Start()
        {
            base.Start();
            father = (ForestBrushPanel)parent;
            height = 30f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoFitChildrenHorizontally = true;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(10, 0, 0, 0);

            SetupDropDown();
            SetupButtons();
        }

        private void SetupDropDown()
        {
            SelectBrushDropDown = AddUIComponent<UIDropDown>();
            SelectBrushDropDown.zOrder = 0;
            SelectBrushDropDown.atlas = ResourceLoader.Atlas;
            SelectBrushDropDown.size = new Vector2(300f, 30f);
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
            SelectBrushDropDown.eventSelectedIndexChanged += EventSelectedIndexChanged;
        }

        internal void LoadBrush(ForestBrush brush)
        {
            UpdateDropDown();
        }

        private void SetupButtons()
        {
            toggleEditButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-EDIT"));
            toggleEditButton.zOrder = 1;
            toggleEditButton.atlas = ForestBrushMod.Instance.Atlas;
            toggleEditButton.normalBgSprite = ResourceLoader.SettingsDropbox;
            toggleEditButton.disabledBgSprite = ResourceLoader.SettingsDropbox;
            toggleEditButton.hoveredBgSprite = ResourceLoader.SettingsDropboxHovered;
            toggleEditButton.focusedBgSprite = ResourceLoader.SettingsDropbox;
            toggleEditButton.pressedBgSprite = ResourceLoader.SettingsDropboxPressed;
            toggleEditButton.eventClicked += OnToggleEditClicked;

            toggleOptionsButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-OPTIONS"));
            toggleOptionsButton.zOrder = 2;
            toggleOptionsButton.atlas = ForestBrushMod.Instance.Atlas;
            toggleOptionsButton.normalBgSprite = ResourceLoader.OptionsDropbox;
            toggleOptionsButton.disabledBgSprite = ResourceLoader.OptionsDropbox;
            toggleOptionsButton.hoveredBgSprite = ResourceLoader.OptionsDropboxHovered;
            toggleOptionsButton.focusedBgSprite = ResourceLoader.OptionsDropbox;
            toggleOptionsButton.pressedBgSprite = ResourceLoader.OptionsDropboxPressed;
            toggleOptionsButton.eventClicked += OnToggleOptionsClicked; 
        }

        private void OnToggleEditClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool editVisible = father.ToggleBrushEdit();
            toggleEditButton.normalBgSprite = toggleEditButton.focusedBgSprite = editVisible ? ResourceLoader.SettingsDropboxPressed : ResourceLoader.SettingsDropbox;
        }

        private void OnToggleOptionsClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool optionsVisible = father.ToggleBrushOptions();
            toggleOptionsButton.normalBgSprite = toggleOptionsButton.focusedBgSprite = optionsVisible ? ResourceLoader.OptionsDropboxPressed : ResourceLoader.OptionsDropbox;
        }

        internal void UpdateDropDown()
        {
            SelectBrushDropDown.items = GetDropdownItems();
            SelectBrushDropDown.eventSelectedIndexChanged -= EventSelectedIndexChanged;
            SelectBrushDropDown.selectedIndex = GetDropdownItemsSelectedIndex();
            SelectBrushDropDown.eventSelectedIndexChanged += EventSelectedIndexChanged;
        }

        private void EventSelectedIndexChanged(UIComponent component, int index)
        {
            var brushName = SelectBrushDropDown.items[index];
            ForestBrushMod.Instance.BrushTool.UpdateTool(brushName);
            father.BrushEditSection.ResetRenameError();
        }

        private string[] GetDropdownItems()
        {
            return ForestBrushMod.instance.Settings.Brushes.Select(b => b.Name).OrderBy(x => x).ToArray();
        }

        private int GetDropdownItemsSelectedIndex()
        {
            return Array.IndexOf(SelectBrushDropDown.items, ForestBrushMod.instance.Settings.SelectedBrush.Name);
        }
    }
}
