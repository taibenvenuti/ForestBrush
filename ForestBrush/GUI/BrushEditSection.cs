using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;
using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushEditSection : UIPanel
    {
        ForestBrushPanel father;
        UIPanel topPanel;
        UIPanel centerPanel;
        UIPanel bottomPanel;
        UIDropDown selectBrushDropDown;
        UITextField renameBrushTextField;
        UIButton createBrushButton;
        UIButton deleteBrushButton;
        internal UIFastList TreesList;
        UILabel searchLabel;
        UITextField SearchTextField;


        public override void Start()
        {
            base.Start();
            father = (ForestBrushPanel)parent;
            selectBrushDropDown = father.BrushSelectSection.SelectBrushDropDown;

            width = father.width;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutStart = LayoutStart.TopLeft;
            autoLayoutPadding = new RectOffset(0, 0, 0, 10);

            topPanel = AddUIComponent<UIPanel>();
            topPanel.height = 30f;
            topPanel.zOrder = 0;
            topPanel.autoLayout = true;
            topPanel.autoLayoutDirection = LayoutDirection.Horizontal;
            topPanel.autoFitChildrenHorizontally = true;
            topPanel.autoLayoutStart = LayoutStart.TopLeft;
            topPanel.autoLayoutPadding = new RectOffset(10, 0, 0, 0);

            centerPanel = AddUIComponent<UIPanel>();
            centerPanel.width = width;
            centerPanel.zOrder = 1;
            centerPanel.autoLayout = true;
            centerPanel.autoLayoutDirection = LayoutDirection.Vertical;
            centerPanel.autoFitChildrenVertically = true;
            centerPanel.autoLayoutStart = LayoutStart.TopLeft;
            centerPanel.autoLayoutPadding = new RectOffset(10, 0, 0, 0);

            bottomPanel = AddUIComponent<UIPanel>();
            bottomPanel.height = 30f;
            bottomPanel.width = 400f;
            bottomPanel.zOrder = 2;

            SetupRenameField();
            SetupButtons();
            SetupListSection();
            SetupSearchFieldSection();
            Hide();
        }

        internal void LoadBrush(ForestBrush brush)
        {
            renameBrushTextField.eventTextChanged -= OnRenameBrushTextChanged;
            renameBrushTextField.text = brush.Name;
            renameBrushTextField.eventTextChanged += OnRenameBrushTextChanged;

            var itemBuffer = TreesList.rows.m_buffer;

            foreach (TreeItem item in itemBuffer)
            {
                item?.UpdateCheckbox();
            }
        }

        private void SetupButtons()
        {
            createBrushButton = UIUtilities.CreateSmallButton(topPanel, Translation.Instance.GetTranslation("FOREST-BRUSH-CREATE"));
            createBrushButton.zOrder = 1;
            createBrushButton.text = "+";
            createBrushButton.eventClicked += OnNewBrushClicked;

            deleteBrushButton = UIUtilities.CreateSmallButton(topPanel, Translation.Instance.GetTranslation("FOREST-BRUSH-DELETE"));
            deleteBrushButton.textPadding = new RectOffset(0, 0, 0, 1);
            deleteBrushButton.zOrder = 2;
            deleteBrushButton.text = "-";
            deleteBrushButton.eventClicked += OnDeleteBrushClicked;
        }

        private void SetupRenameField()
        {
            renameBrushTextField = topPanel.AddUIComponent<UITextField>();
            renameBrushTextField.zOrder = 0;
            renameBrushTextField.atlas = ResourceLoader.Atlas;
            renameBrushTextField.size = new Vector2(300f, 30f);
            renameBrushTextField.padding = new RectOffset(6, 6, 6, 6);
            renameBrushTextField.builtinKeyNavigation = true;
            renameBrushTextField.isInteractive = true;
            renameBrushTextField.readOnly = false;
            renameBrushTextField.horizontalAlignment = UIHorizontalAlignment.Center;
            renameBrushTextField.selectionSprite = ResourceLoader.EmptySprite;
            renameBrushTextField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            renameBrushTextField.normalBgSprite = ResourceLoader.TextFieldPanelHovered;
            renameBrushTextField.disabledBgSprite = ResourceLoader.TextFieldPanelHovered;
            renameBrushTextField.textColor = new Color32(0, 0, 0, 255);
            renameBrushTextField.disabledTextColor = new Color32(80, 80, 80, 128);
            renameBrushTextField.color = new Color32(255, 255, 255, 255);
            renameBrushTextField.relativePosition = new Vector3(width - Constants.UISpacing - renameBrushTextField.width, 560f);
            renameBrushTextField.text = UserMod.Settings.SelectedBrush.Name;
            renameBrushTextField.eventTextChanged += OnRenameBrushTextChanged;
            renameBrushTextField.eventKeyPress += OnRenameBrushKeyPress;
            renameBrushTextField.eventLostFocus += (c, e) => UserMod.SaveSettings();
            renameBrushTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-RENAME-BRUSH");
        }

        private void SetupListSection()
        {
            TreesList = UIFastList.Create<TreeItem>(centerPanel);
            TreesList.zOrder = 0;
            TreesList.backgroundSprite = "UnlockingPanel";
            TreesList.size = new Vector2(width - Constants.UISpacing * 2, 420f);
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
        }

        private void SetupSearchFieldSection()
        {
            searchLabel = bottomPanel.AddUIComponent<UILabel>();
            searchLabel.zOrder = 0;
            searchLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-SEARCH");
            searchLabel.padding = new RectOffset(0, 0, 8, 0);
            searchLabel.textScale = Constants.UITitleTextScale;
            searchLabel.verticalAlignment = UIVerticalAlignment.Middle;
            searchLabel.textAlignment = UIHorizontalAlignment.Right;
            searchLabel.autoSize = false;
            searchLabel.size = new Vector2(150f, 30f);
            searchLabel.relativePosition = new Vector3(0f, 0f);

            SearchTextField = bottomPanel.AddUIComponent<UITextField>();
            SearchTextField.zOrder = 1;
            SearchTextField.atlas = ResourceLoader.Atlas;
            SearchTextField.size = new Vector2(230f, 30f);
            SearchTextField.relativePosition = new Vector3(160f, 0f);
            SearchTextField.padding = new RectOffset(6, 6, 6, 6);
            SearchTextField.builtinKeyNavigation = true;
            SearchTextField.isInteractive = true;
            SearchTextField.readOnly = false;
            SearchTextField.horizontalAlignment = UIHorizontalAlignment.Center;
            SearchTextField.selectionSprite = ResourceLoader.EmptySprite;
            SearchTextField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            SearchTextField.normalBgSprite = ResourceLoader.TextFieldPanelHovered;
            SearchTextField.disabledBgSprite = ResourceLoader.TextFieldPanelHovered;
            SearchTextField.textColor = new Color32(0, 0, 0, 255);
            SearchTextField.disabledTextColor = new Color32(80, 80, 80, 128);
            SearchTextField.color = new Color32(255, 255, 255, 255);
            SearchTextField.eventTextChanged += OnSearchTextChanged;
            SearchTextField.eventKeyDown += (c, e) =>
            {
                if (e.keycode == KeyCode.Escape)
                {
                    SearchTextField.text = "";
                    SearchTextField.Unfocus();
                }
            };
        }

        private void OnNewBrushClicked(UIComponent component, UIMouseEventParameter mouseEventParameter)
        {
            string name = Constants.NewBrushName;
            int suffix = 0;
            while (UserMod.Settings.Brushes.Find(b => b.Name == name) != null)
            {
                name = $"{Constants.NewBrushName}{suffix}";
                suffix++;
                if (suffix >= 99999)
                {
                    Debug.LogError("Infinite loop detected while trying to generate new name. Please contact the mod author.");
                    return;
                }
            }
            ForestBrushMod.Instance.BrushTool.New(name);
            renameBrushTextField.eventTextChanged -= OnRenameBrushTextChanged;
            renameBrushTextField.text = name;
            renameBrushTextField.Focus();
            renameBrushTextField.SelectAll();
            renameBrushTextField.eventTextChanged += OnRenameBrushTextChanged;
        }

        private void OnRenameBrushKeyPress(UIComponent component, UIKeyEventParameter parameter)
        {
            char ch = parameter.character;
            if (!char.IsControl(ch) && !char.IsWhiteSpace(ch) && !char.IsLetterOrDigit(ch))
            {
                parameter.Use();
            }
            if (parameter.keycode == KeyCode.Escape)
            {
                renameBrushTextField.Unfocus();
            }
        }

        private void OnDeleteBrushClicked(UIComponent component, UIMouseEventParameter mouseEventParameter)
        {            
            ConfirmPanel.ShowModal(Translation.Instance.GetTranslation("FOREST-BRUSH-MODAL-WARNING"), Translation.Instance.GetTranslation("FOREST-BRUSH-PROMPT-DELETE"), (d, i) =>
            {
                if (i == 1)
                {
                    ForestBrushMod.Instance.BrushTool.DeleteCurrent();
                }
            });
        }

        private void OnRenameBrushTextChanged(UIComponent component, string newName)
        {
            string currentName = ForestBrushMod.Instance.BrushTool.Brush.Name;
            if (UserMod.Settings.Brushes.Find(b => b.Name == newName && b.Name != currentName) == null)
            {
                ResetRenameError();
                UIDropDown brushDropDown = father.BrushSelectSection.SelectBrushDropDown;
                if (newName != brushDropDown.items[brushDropDown.selectedIndex])
                {
                    brushDropDown.items[brushDropDown.selectedIndex] = newName;
                }
                ForestBrushMod.Instance.BrushTool.Brush.Name = newName;
                brushDropDown.Invalidate();
            }
            else
            {
                SetRenameError();
            }
        }

        private void SetRenameError()
        {
            renameBrushTextField.textColor = Color.red;
            renameBrushTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-RENAME-ERROR");
        }

        public void ResetRenameError()
        {
            renameBrushTextField.textColor = new Color32(0, 0, 0, 255);
            renameBrushTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-RENAME-BRUSH");
        }

        private void OnSearchTextChanged(UIComponent component, string text)
        {
            string[] filters = text?.Trim()?.ToLower().Split(' ');
            if (TreesList == null || ForestBrushMod.Instance.Trees == null) return;
            var data = ForestBrushMod.Instance.Trees.Values.ToList();
            if (filters != null && filters.Length > 0 && !string.IsNullOrEmpty(filters[0]))
            {
                var newData = new List<TreeInfo>();
                for (int i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        for (int j = 0; j < data.Count; j++)
                        {
                            var item = data[j];
                            if (item == null) continue;
                            if (item.GetLocalizedDescription().ToLower().Contains(filter)
                                || item.GetUncheckedLocalizedTitle().ToLower().Contains(filter))
                            {
                                newData.Add(item);
                            }
                        }
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
            List<TreeInfo> trees = ForestBrushMod.Instance.Trees.Values.ToList();
            trees.Sort((t1, t2) => t1.GetUncheckedLocalizedTitle().CompareTo(t2.GetUncheckedLocalizedTitle()));
            return trees;
        }

        internal void FocusSearchField()
        {
            SearchTextField.Focus();
        }
    }
}
