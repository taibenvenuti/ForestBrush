using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class ForestBrushPanel : UIPanel
    {
        UIDragHandle dragHandle;
        UILabel titleLabel;
        UIButton closeButton;
        UIDropDown brushesDropDown;
        internal UIFastList TreesList;
        UIButton saveCurrentButton;
        UIButton saveNewButton;
        UIButton deleteButton;
        UIButton brushOptionsButton;
        UILabel searchLabel;
        internal UITextField SearchTextField;
        public BrushOptionsPanel brushOptionsPanel;

        public override void Start()
        {
            base.Start();
            Setup();
        }

        private void Setup()
        {
            size = Constants.UIPanelSize;
            relativePosition = new Vector3(CGSSerialized.PanelX, CGSSerialized.PanelY);
            atlas = UIUtilities.GetAtlas();
            backgroundSprite = "MenuPanel";
            isVisible = true;
            isInteractive = true;

            brushOptionsPanel = AddUIComponent<BrushOptionsPanel>();

            titleLabel = AddUIComponent<UILabel>();
            titleLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-MODNAME");
            titleLabel.textScale = Constants.UITextScale;
            titleLabel.relativePosition = new Vector3((width - titleLabel.width) / 2f, (Constants.UITitleBarHeight - titleLabel.height) / 2f);

            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(width, Constants.UITitleBarHeight);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;
            dragHandle.eventMouseUp += (c, e) => SavePanelPosition();

            closeButton = AddUIComponent<UIButton>();
            closeButton.atlas = UIUtilities.GetAtlas();
            closeButton.size = new Vector2(20f, 20f);
            closeButton.relativePosition = new Vector3(width - closeButton.width - Constants.UISpacing, Constants.UISpacing);
            closeButton.normalBgSprite = "DeleteLineButton";
            closeButton.hoveredBgSprite = "DeleteLineButtonHovered";
            closeButton.pressedBgSprite = "DeleteLineButtonPressed";
            closeButton.eventClick += (component, param) =>
            {
                ForestBrushMod.instance.ToggleButton.SimulateClick();
            };

            TreesList = UIFastList.Create<TreeItem>(this);
            TreesList.backgroundSprite = "UnlockingPanel";
            TreesList.relativePosition = new Vector3(Constants.UISpacing, 130f);
            TreesList.size = new Vector2(width - Constants.UISpacing * 2, 460f);
            List<TreeInfo> trees = GetAvailableTreesSorted();
            TreesList.rowHeight = Constants.UIItemHeight;
            if (trees.Count > 0)
            {
                for (int i = 0; i < trees.Count; i++)
                {
                    if (trees[i] != null)
                        TreesList.rowsData.Add(trees[i]);
                }
                TreesList.DisplayAt(0f);
            }
            TreesList.selectedIndex = -1;

            brushesDropDown = AddUIComponent<UIDropDown>();
            brushesDropDown.atlas = UIUtilities.GetAtlas();
            brushesDropDown.size = new Vector2(Constants.UIPanelSize.x - Constants.UISpacing * 2, Constants.UIButtonHeight);
            brushesDropDown.relativePosition = new Vector3(Constants.UISpacing, Constants.UITitleBarHeight + Constants.UISpacing);
            brushesDropDown.items = UserMod.BrushSettings.SavedBrushes.Select(b => b.Name).ToArray();
            brushesDropDown.listBackground = "StylesDropboxListbox";
            brushesDropDown.itemHeight = (int)Constants.UIButtonHeight;
            brushesDropDown.itemHover = "ListItemHover";
            brushesDropDown.itemHighlight = "ListItemHighlight";
            brushesDropDown.normalBgSprite = "CMStylesDropbox";
            brushesDropDown.hoveredBgSprite = "CMStylesDropboxHovered";
            brushesDropDown.disabledBgSprite = "";
            brushesDropDown.focusedBgSprite = "";
            brushesDropDown.listWidth = (int)(Constants.UIPanelSize.x - Constants.UISpacing * 2);
            brushesDropDown.listHeight = 500;
            brushesDropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            brushesDropDown.popupColor = Color.white;
            brushesDropDown.popupTextColor = new Color32(170, 170, 170, 255);
            brushesDropDown.zOrder = 1;
            brushesDropDown.textScale = Constants.UITextScale;
            brushesDropDown.verticalAlignment = UIVerticalAlignment.Middle;
            brushesDropDown.horizontalAlignment = UIHorizontalAlignment.Left;
            brushesDropDown.selectedIndex = brushesDropDown.items.ToList().FindIndex(i => i == CGSSerialized.SelectedBrush) == -1 ? 0 : brushesDropDown.items.ToList().FindIndex(i => i == CGSSerialized.SelectedBrush);
            brushesDropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            brushesDropDown.itemPadding = new RectOffset(14, 0, 8, 0);
            brushesDropDown.triggerButton = brushesDropDown;
            brushesDropDown.eventSelectedIndexChanged += EventSelectedIndexChanged;

            saveCurrentButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-SAVECURRENT"));
            saveCurrentButton.relativePosition = new Vector3(Constants.UISpacing, Constants.UITitleBarHeight + Constants.UIButtonHeight + Constants.UISpacing * 2);
            saveCurrentButton.eventClicked += (c, e) => OnSaveCurrentClickedEventHandler(CGSSerialized.ConfirmOverwrite);

            saveNewButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-SAVENEW"));
            saveNewButton.relativePosition = new Vector3(Constants.UIButtonWidth + Constants.UISpacing * 2, Constants.UITitleBarHeight + Constants.UIButtonHeight + Constants.UISpacing * 2);
            saveNewButton.eventClicked += (c, e) => UIView.PushModal(AddUIComponent<NewBrushModal>());

            deleteButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-DELETE"));
            deleteButton.relativePosition = new Vector3(Constants.UIButtonWidth * 2 + Constants.UISpacing * 3, Constants.UITitleBarHeight + Constants.UIButtonHeight + Constants.UISpacing * 2);
            deleteButton.eventClicked += (c, e) =>
            {
                if (brushesDropDown.selectedValue == Constants.VanillaPack)
                {
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                        UserMod.Translation.GetTranslation("FOREST-BRUSH-MODAL-MESSAGE"),
                        UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-ERROR"),
                        false);
                    return;
                }
                else
                {
                    ConfirmPanel.ShowModal(UserMod.Translation.GetTranslation("FOREST-BRUSH-MODAL-WARNING"), UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-DELETE"), (d, i) =>
                    {
                        if (i == 1)
                        {
                            ForestBrushMod.instance.BrushTool.DeleteCurrent();
                        }
                    });
                }        
            };

            SearchTextField = AddUIComponent<UITextField>();
            SearchTextField.atlas = UIUtilities.GetAtlas();
            SearchTextField.size = new Vector2(150f, 30f);
            SearchTextField.padding = new RectOffset(6, 6, 6, 6);
            SearchTextField.builtinKeyNavigation = true;
            SearchTextField.isInteractive = true;
            SearchTextField.readOnly = false;
            SearchTextField.horizontalAlignment = UIHorizontalAlignment.Center;
            SearchTextField.selectionSprite = "EmptySprite";
            SearchTextField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            SearchTextField.normalBgSprite = "TextFieldPanelHovered";
            SearchTextField.disabledBgSprite = "TextFieldPanelHovered";
            SearchTextField.textColor = new Color32(0, 0, 0, 255);
            SearchTextField.disabledTextColor = new Color32(80, 80, 80, 128);
            SearchTextField.color = new Color32(255, 255, 255, 255);
            SearchTextField.relativePosition = new Vector3(width - Constants.UISpacing - SearchTextField.width, 600f);
            SearchTextField.eventTextChanged += OnSearchTextChanged;
            SearchTextField.eventKeyUp += (c, e) =>
            {
                if (e.keycode == KeyCode.Escape)
                {
                    SearchTextField.text = "";
                    SearchTextField.Unfocus();
                }
            };

            searchLabel = AddUIComponent<UILabel>();
            searchLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-SEARCH");
            searchLabel.textScale = Constants.UITextScale;
            searchLabel.relativePosition = new Vector3(SearchTextField.relativePosition.x - searchLabel.width - Constants.UISpacing, SearchTextField.relativePosition.y + ((30f - searchLabel.height) / 2));

            brushOptionsButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-BUTTON"));
            brushOptionsButton.relativePosition = new Vector3(Constants.UISpacing, SearchTextField.relativePosition.y);
            brushOptionsButton.eventClicked += (c, e) =>
            {
                brushOptionsPanel.isVisible = !brushOptionsPanel.isVisible;
            };

            Hide();
        }

        public void SavePanelPosition()
        {
            CGSSerialized.PanelX.value = relativePosition.x;
            CGSSerialized.PanelY.value = relativePosition.y;
        }

        internal void UpdateDropDown()
        {
            brushesDropDown.items = UserMod.BrushSettings.SavedBrushes.Select(b => b.Name).ToArray();
            int index = brushesDropDown.items.ToList().FindIndex(i => i == CGSSerialized.SelectedBrush);
            brushesDropDown.selectedIndex = index == -1 ? 0 : index;
        }

        internal void OnSaveCurrentClickedEventHandler(bool confirm)
        {
            if (brushesDropDown.selectedValue == Constants.VanillaPack)
            {
                UIView.PushModal(ForestBrushMod.instance.ForestBrushPanel.AddUIComponent<NewBrushModal>());
                return;
            }
            else
            {
                if (confirm)                    
                ConfirmPanel.ShowModal(UserMod.Translation.GetTranslation("FOREST-BRUSH-MODAL-WARNING"), UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-OVERWRITE"), (d, i) => {
                        if (i == 1)
                        {
                            ForestBrushMod.instance.BrushTool.Save();
                        }
                    });
                else ForestBrushMod.instance.BrushTool.Save();
            }
            UpdateDropDown();
        }

        private void OnSearchTextChanged(UIComponent component, string text)
        {            
            string filter = text?.Trim()?.ToLower();
            if (TreesList == null || ForestBrushMod.instance.Trees == null) return;
            var data = ForestBrushMod.instance.Trees.Values.ToList();
            if (!String.IsNullOrEmpty(filter))
            {
                var newData = new List<TreeInfo>();
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    if (item == null) continue;
                    var itemName = item.GetUncheckedLocalizedTitle().Trim().ToLower();
                    if (itemName.Contains(filter) || item.name.Replace("_Data", "").Trim().ToLower().Contains(filter))
                    {
                        newData.Add(item);
                    }
                }
                data = newData;
            }
            data.Sort((t1, t2) => t1.GetUncheckedLocalizedTitle().CompareTo(t2.GetUncheckedLocalizedTitle()));
            TreesList.rowsData.m_buffer = data.ToArray();
            TreesList.rowsData.m_size = data.Count;
            TreesList.DisplayAt(0f);
        }        

        private List<TreeInfo> GetAvailableTreesSorted()
        {
            List<TreeInfo> trees = ForestBrushMod.instance.Trees.Values.ToList();
            trees.Sort((t1, t2) => t1.GetUncheckedLocalizedTitle().CompareTo(t2.GetUncheckedLocalizedTitle()));
            return trees;
        }

        private void EventSelectedIndexChanged(UIComponent component, int index)
        {
            if (index >= brushesDropDown.items.Length) return;
            var key = brushesDropDown.items[index];
            if (UserMod.BrushSettings.SavedBrushes.Find(b => b.Name == key) != null)
            {
                ForestBrushMod.instance.BrushTool.UpdateTool(key);
                var itemBuffer = ForestBrushMod.instance.ForestBrushPanel.TreesList.rows.m_buffer;
                foreach (TreeItem item in itemBuffer)
                {
                    item?.UpdateCheckbox();
                }
            }
        }
    }
}