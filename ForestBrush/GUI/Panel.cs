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
        UITextField searchTextField;
        public BrushOptionsPanel brushOptionsPanel;

        public override void Start()
        {
            base.Start();
            Setup();
        }

        private void Setup()
        {
            size = Constants.UIPanelSize;
            relativePosition = new Vector3(UserMod.Settings.PanelX, UserMod.Settings.PanelY);
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
            dragHandle.eventMouseUp += (c, e) =>
            {
                UserMod.Settings.PanelX = relativePosition.x;
                UserMod.Settings.PanelY = relativePosition.y;
                UserMod.Settings.Save();
            };

            closeButton = AddUIComponent<UIButton>();
            closeButton.atlas = UIUtilities.GetAtlas();
            closeButton.size = new Vector2(20f, 20f);
            closeButton.relativePosition = new Vector3(width - closeButton.width - Constants.UISpacing, Constants.UISpacing);
            closeButton.normalBgSprite = "DeleteLineButton";
            closeButton.hoveredBgSprite = "DeleteLineButtonHovered";
            closeButton.pressedBgSprite = "DeleteLineButtonPressed";
            closeButton.eventClick += (component, param) =>
            {
                isVisible = false;
            };

            TreesList = UIFastList.Create<TreeItem>(this);
            TreesList.backgroundSprite = "UnlockingPanel";
            TreesList.relativePosition = new Vector3(Constants.UISpacing, 130f);
            TreesList.size = new Vector2(width - Constants.UISpacing * 2, 660f);
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
            brushesDropDown.items = ForestBrushes.instance.Brushes.Keys.ToArray();
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
            brushesDropDown.selectedIndex = brushesDropDown.items.ToList().FindIndex(i => i == UserMod.Settings.SelectedBrush) == -1 ? 0 : brushesDropDown.items.ToList().FindIndex(i => i == UserMod.Settings.SelectedBrush);
            brushesDropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            brushesDropDown.itemPadding = new RectOffset(14, 0, 8, 0);
            brushesDropDown.triggerButton = brushesDropDown;
            brushesDropDown.eventSelectedIndexChanged += EventSelectedIndexChanged;

            saveCurrentButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-SAVECURRENT"));
            saveCurrentButton.relativePosition = new Vector3(Constants.UISpacing, Constants.UITitleBarHeight + Constants.UIButtonHeight + Constants.UISpacing * 2);
            saveCurrentButton.eventClicked += (c, e) => OnSaveCurrentClickedEventHandler(UserMod.Settings.ConfirmOverwrite);

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
                            ForestBrushes.instance.BrushTool.Delete();
                        }
                    });
                }        
            };

            searchTextField = AddUIComponent<UITextField>();
            searchTextField.atlas = UIUtilities.GetAtlas();
            searchTextField.size = new Vector2(150f, 30f);
            searchTextField.padding = new RectOffset(6, 6, 6, 6);
            searchTextField.builtinKeyNavigation = true;
            searchTextField.isInteractive = true;
            searchTextField.readOnly = false;
            searchTextField.horizontalAlignment = UIHorizontalAlignment.Center;
            searchTextField.selectionSprite = "EmptySprite";
            searchTextField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            searchTextField.normalBgSprite = "TextFieldPanelHovered";
            searchTextField.disabledBgSprite = "TextFieldPanelHovered";
            searchTextField.textColor = new Color32(0, 0, 0, 255);
            searchTextField.disabledTextColor = new Color32(80, 80, 80, 128);
            searchTextField.color = new Color32(255, 255, 255, 255);
            searchTextField.relativePosition = new Vector3(width - Constants.UISpacing - searchTextField.width, 800f);
            searchTextField.eventTextChanged += OnSearchTextChanged;

            searchLabel = AddUIComponent<UILabel>();
            searchLabel.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-SEARCH");
            searchLabel.textScale = Constants.UITextScale;
            searchLabel.relativePosition = new Vector3(searchTextField.relativePosition.x - searchLabel.width - Constants.UISpacing, searchTextField.relativePosition.y + ((30f - searchLabel.height) / 2));

            brushOptionsButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-BRUSH-OPTIONS-BUTTON"));
            brushOptionsButton.relativePosition = new Vector3(Constants.UISpacing, searchTextField.relativePosition.y);
            brushOptionsButton.eventClicked += (c, e) =>
            {
                brushOptionsPanel.isVisible = !brushOptionsPanel.isVisible;
            };

            ForestBrushPerks.Apply();
        }

        internal void UpdateDropDown()
        {
            brushesDropDown.items = ForestBrushes.instance.Brushes.Keys.ToArray();
            brushesDropDown.selectedIndex = brushesDropDown.items.ToList().FindIndex(i => i == UserMod.Settings.SelectedBrush) == -1 ? 0 : brushesDropDown.items.ToList().FindIndex(i => i == UserMod.Settings.SelectedBrush);
        }

        internal void OnSaveCurrentClickedEventHandler(bool confirm)
        {
            if (brushesDropDown.selectedValue == Constants.VanillaPack)
            {
                UIView.PushModal(ForestBrushes.instance.BrushPanel.AddUIComponent<NewBrushModal>());
                return;
            }
            else
            {
                if (confirm)                    
                ConfirmPanel.ShowModal(UserMod.Translation.GetTranslation("FOREST-BRUSH-MODAL-WARNING"), UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-OVERWRITE"), (d, i) => {
                        if (i == 1)
                        {
                            ForestBrushes.instance.BrushTool.Save();
                        }
                    });
                else ForestBrushes.instance.BrushTool.Save();
            }
        }

        private void OnSearchTextChanged(UIComponent component, string text)
        {            
            string filter = text?.Trim()?.ToLower();
            if (TreesList == null || ForestBrushes.instance.Trees == null) return;
            var data = ForestBrushes.instance.Trees.Values.ToList();
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
            List<TreeInfo> trees = ForestBrushes.instance.Trees.Values.ToList();
            trees.Sort((t1, t2) => t1.GetUncheckedLocalizedTitle().CompareTo(t2.GetUncheckedLocalizedTitle()));
            return trees;
        }

        private void EventSelectedIndexChanged(UIComponent component, int index)
        {
            if (index >= brushesDropDown.items.Length) return;
            var key = brushesDropDown.items[index];
            if (ForestBrushes.instance.Brushes.TryGetValue(key, out List<string> value))
                ForestBrushes.instance.BrushTool = new TreeBrushTool(key, value);
            else Debug.LogWarning($"{key} Key not found in {ForestBrushes.instance.Brushes}");
            var itemBuffer = ForestBrushes.instance.BrushPanel.TreesList.rows.m_buffer;
            foreach (TreeItem item in itemBuffer)
            {
                item?.UpdateCheckbox();
            }
        }
    }
}