using ColossalFramework.UI;
using ForestBrush.TranslationFramework;
using System;
using System.Linq;
using UnityEngine;
using static ForestBrush.Resources.ResourceLoader;

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
            SelectBrushDropDown.atlas = GetAtlas("Ingame");
            SelectBrushDropDown.size = new Vector2(300f, 30f);
            SelectBrushDropDown.items = GetDropdownItems();
            SelectBrushDropDown.listBackground = "StylesDropboxListbox";
            SelectBrushDropDown.itemHeight = (int)Constants.UIButtonHeight;
            SelectBrushDropDown.itemHover = "ListItemHover";
            SelectBrushDropDown.itemHighlight = "ListItemHighlight";
            SelectBrushDropDown.normalBgSprite = "CMStylesDropbox";
            SelectBrushDropDown.hoveredBgSprite = "CMStylesDropboxHovered";
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
            toggleEditButton.atlas = ForestBrushMod.instance.Atlas;
            toggleEditButton.normalBgSprite = SettingsDropbox;
            toggleEditButton.disabledBgSprite = SettingsDropbox;
            toggleEditButton.hoveredBgSprite = SettingsDropboxHovered;
            toggleEditButton.focusedBgSprite = SettingsDropbox;
            toggleEditButton.pressedBgSprite = SettingsDropboxPressed;
            toggleEditButton.eventClicked += OnToggleEditClicked;

            toggleOptionsButton = UIUtilities.CreateSmallButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-TOGGLE-OPTIONS"));
            toggleOptionsButton.zOrder = 2;
            toggleOptionsButton.atlas = ForestBrushMod.instance.Atlas;
            toggleOptionsButton.normalBgSprite = OptionsDropbox;
            toggleOptionsButton.disabledBgSprite = OptionsDropbox;
            toggleOptionsButton.hoveredBgSprite = OptionsDropboxHovered;
            toggleOptionsButton.focusedBgSprite = OptionsDropbox;
            toggleOptionsButton.pressedBgSprite = OptionsDropboxPressed;
            toggleOptionsButton.eventClicked += OnToggleOptionsClicked; 
        }

        private void OnToggleEditClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool editVisible = father.ToggleBrushEdit();
            toggleEditButton.normalBgSprite = toggleEditButton.focusedBgSprite = editVisible ?  SettingsDropboxPressed : SettingsDropbox;
        }

        private void OnToggleOptionsClicked(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool optionsVisible = father.ToggleBrushOptions();
            toggleOptionsButton.normalBgSprite = toggleOptionsButton.focusedBgSprite = optionsVisible ? OptionsDropboxPressed : OptionsDropbox;
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
            ForestBrushMod.instance.BrushTool.UpdateTool(brushName);
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
