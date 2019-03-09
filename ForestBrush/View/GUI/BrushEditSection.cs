using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class BrushEditSection : UIPanel
    {
        ForestBrushPanel owner;
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
        internal Filtering Filtering;

        public override void Start()
        {
            base.Start();
            owner = (ForestBrushPanel)parent;
            selectBrushDropDown = owner.BrushSelectSection.SelectBrushDropDown;

            Filtering = new Filtering();

            width = owner.width;
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

            LoadBrush(UserMod.Settings.SelectedBrush);

            if (!UserMod.Settings.BrushEditOpen)
            {
                owner.BrushSelectSection.UnfocusEditSectionButton();
                Hide();
            }
        }

        public override void OnDestroy()
        {
            renameBrushTextField.eventTextChanged -= RenameBrushTextField_eventTextChanged;
            renameBrushTextField.eventKeyPress -= RenameBrushTextField_eventKeyPress;
            renameBrushTextField.eventLostFocus -= RenameBrushTextField_eventLostFocus;
            createBrushButton.eventClicked -= CreateBrushButton_eventClicked;
            deleteBrushButton.eventClicked -= DeleteBrushButton_eventClicked;
            SearchTextField.eventTextChanged -= SearchTextField_eventTextChanged;
            SearchTextField.eventLostFocus -= SearchTextField_eventLostFocus;
            base.OnDestroy();
        }

        private void RenameBrushTextField_eventLostFocus(UIComponent component, UIFocusEventParameter eventParam)
        {
            UserMod.SaveSettings();
        }

        internal void LocaleChanged()
        {
            createBrushButton.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-CREATE");
            deleteBrushButton.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-DELETE");
            renameBrushTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-RENAME-BRUSH");
            searchLabel.text = Translation.Instance.GetTranslation("FOREST-BRUSH-SEARCH");
            foreach (TreeItem item in TreesList.rowsData)
            {
                item?.LocaleChanged();
            }
        }

        internal void LoadBrush(Brush brush)
        {
            renameBrushTextField.eventTextChanged -= RenameBrushTextField_eventTextChanged;
            renameBrushTextField.text = brush.Name;
            renameBrushTextField.eventTextChanged += RenameBrushTextField_eventTextChanged;

            foreach (TreeItem item in TreesList.rows)
            {
                item?.UpdateCheckbox();
            }
        }

        private void SetupButtons()
        {
            createBrushButton = UIUtilities.CreateSmallButton(topPanel, Translation.Instance.GetTranslation("FOREST-BRUSH-CREATE"));
            createBrushButton.zOrder = 1;
            createBrushButton.text = "+";
            createBrushButton.eventClicked += CreateBrushButton_eventClicked;

            deleteBrushButton = UIUtilities.CreateSmallButton(topPanel, Translation.Instance.GetTranslation("FOREST-BRUSH-DELETE"));
            deleteBrushButton.textPadding = new RectOffset(0, 0, 0, 1);
            deleteBrushButton.zOrder = 2;
            deleteBrushButton.text = "-";
            deleteBrushButton.eventClicked += DeleteBrushButton_eventClicked;
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
            renameBrushTextField.eventTextChanged += RenameBrushTextField_eventTextChanged;
            renameBrushTextField.eventKeyPress += RenameBrushTextField_eventKeyPress;
            renameBrushTextField.eventLostFocus += RenameBrushTextField_eventLostFocus;
            renameBrushTextField.tooltip = Translation.Instance.GetTranslation("FOREST-BRUSH-RENAME-BRUSH");
        }

        private void SetupListSection()
        {
            TreesList = UIFastList.Create<TreeItem>(centerPanel);
            TreesList.zOrder = 0;
            TreesList.backgroundSprite = "UnlockingPanel";
            TreesList.size = new Vector2(width - Constants.UISpacing * 2, 420f);
            SetupFastlist();
        }

        internal void SetupFastlist()
        {
            TreesList.rowsData.Clear();
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
            TreesList.Refresh();
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
            SearchTextField.eventTextChanged += SearchTextField_eventTextChanged;
            SearchTextField.eventLostFocus += SearchTextField_eventLostFocus;
        }

        private void CreateBrushButton_eventClicked(UIComponent component, UIMouseEventParameter mouseEventParameter)
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
            ForestBrush.Instance.BrushTool.New(name);
            renameBrushTextField.eventTextChanged -= RenameBrushTextField_eventTextChanged;
            renameBrushTextField.text = name;
            renameBrushTextField.Focus();
            renameBrushTextField.SelectAll();
            renameBrushTextField.eventTextChanged += RenameBrushTextField_eventTextChanged;
        }

        private void RenameBrushTextField_eventKeyPress(UIComponent component, UIKeyEventParameter parameter)
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

        private void DeleteBrushButton_eventClicked(UIComponent component, UIMouseEventParameter mouseEventParameter)
        {
            ConfirmPanel.ShowModal(Translation.Instance.GetTranslation("FOREST-BRUSH-MODAL-WARNING"), Translation.Instance.GetTranslation("FOREST-BRUSH-PROMPT-DELETE"), (d, i) =>
            {
                if (i == 1)
                {
                    ForestBrush.Instance.BrushTool.DeleteCurrent();
                }
            });
        }

        private void RenameBrushTextField_eventTextChanged(UIComponent component, string newName)
        {
            string currentName = ForestBrush.Instance.BrushTool.Brush.Name;
            if (UserMod.Settings.Brushes.Find(b => b.Name == newName && b.Name != currentName) == null)
            {
                ResetRenameError();
                UIDropDown brushDropDown = owner.BrushSelectSection.SelectBrushDropDown;
                if (newName != brushDropDown.items[brushDropDown.selectedIndex])
                {
                    brushDropDown.items[brushDropDown.selectedIndex] = newName;
                }
                ForestBrush.Instance.BrushTool.Brush.Name = newName;
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

        private void SearchTextField_eventTextChanged(UIComponent component, string text)
        {
            switch (UserMod.Settings.FilterStyle)
            {
                case FilterStyle.Basic:
                    Filtering.FilterTreeListBasic(text);
                    break;
                case FilterStyle.AND:
                    Filtering.FilterTreeListExclusive(text);
                    break;
                case FilterStyle.OR:
                    Filtering.FilterTreeListInclusive(text);
                    break;
            }
        }

        private void SearchTextField_eventLostFocus(UIComponent component, UIFocusEventParameter eventParam)
        {
            switch (UserMod.Settings.FilterStyle)
            {
                case FilterStyle.Basic:
                    Filtering.FilterTreeListBasic(SearchTextField.text);
                    break;
                case FilterStyle.AND:
                    Filtering.FilterTreeListExclusive(SearchTextField.text);
                    break;
                case FilterStyle.OR:
                    Filtering.FilterTreeListInclusive(SearchTextField.text);
                    break;
            }
        }

        private bool IsTextureFilter(string filter)
        {
            return filter.Length > 2 && filter.Substring(filter.Length - 2).ToLower() == "px";
        }

        private bool IsTriangleFilter(string filter)
        {
            return filter.Length > 4 && filter.Substring(filter.Length - 4).ToLower() == "tris";
        }

        private List<TreeInfo> GetAvailableTreesSorted()
        {
            List<TreeInfo> trees = ForestBrush.Instance.Trees.Values.ToList();
            trees.Sort((t1, t2) => t1.CompareTo(t2, UserMod.Settings.Sorting, UserMod.Settings.SortingOrder));
            return trees;
        }

        internal void FocusSearchField()
        {
            SearchTextField.Focus();
        }
    }
}
