using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class TreeItem : UIPanel, IUIFastListRow
    {
        TreeInfo treeInfo;
        UICheckBox includeCheckBox;
        UILabel treeNameLabel;
        UITextureSprite thumbNailSprite;
        bool initialized;
        public string Name => treeNameLabel?.text;

        private void Initialize(TreeInfo info)
        {
            if (initialized) return;

            treeInfo = info;

            //General
            atlas = UIUtilities.GetAtlas();
            width = parent.width;
            height = Constants.UIItemHeight;
            isVisible = true;
            isInteractive = true;

            //Thumbnail
            thumbNailSprite = AddUIComponent<UITextureSprite>();
            thumbNailSprite.texture = treeInfo.m_Atlas.sprites.Find(spr => spr.name == treeInfo.m_Thumbnail).texture;
            thumbNailSprite.size = new Vector2(50f, 54.5f);
            thumbNailSprite.relativePosition = new Vector3(Constants.UISpacing, (Constants.UIItemHeight - thumbNailSprite.height) / 2);

            //CheckBox
            includeCheckBox = AddUIComponent<UICheckBox>();
            includeCheckBox.size = Constants.UICheckboxSize;
            var sprite = includeCheckBox.AddUIComponent<UISprite>();
            sprite.atlas = UIUtilities.GetAtlas();
            sprite.spriteName = "ToggleBase";
            sprite.size = includeCheckBox.size;
            sprite.relativePosition = Vector3.zero;
            includeCheckBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)includeCheckBox.checkedBoxObject).atlas = UIUtilities.GetAtlas();
            ((UISprite)includeCheckBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            includeCheckBox.checkedBoxObject.size = includeCheckBox.size;
            includeCheckBox.checkedBoxObject.relativePosition = Vector3.zero;
            includeCheckBox.eventCheckChanged += EventIncludeTree;
            includeCheckBox.isChecked = ForestBrushes.instance.BrushTool.Container.m_variations.Any(v => v.m_finalTree == treeInfo);
            includeCheckBox.relativePosition = new Vector3(width - (Constants.UISpacing * 2) - includeCheckBox.width, (height - includeCheckBox.height) / 2);

            //Label
            treeNameLabel = AddUIComponent<UILabel>();
            treeNameLabel.text = treeInfo.GetUncheckedLocalizedTitle();
            treeNameLabel.relativePosition = new Vector3(thumbNailSprite.width + Constants.UISpacing * 2, (height - treeNameLabel.height) / 2);

            initialized = true;
        }

        public void ToggleCheckbox(bool value)
        {
            includeCheckBox.isChecked = value;
        }

        public void UpdateCheckbox()
        {
            includeCheckBox.isChecked = ForestBrushes.instance.BrushTool.Container.m_variations.Any(v => v.m_finalTree == treeInfo);
        }

        private void EventIncludeTree(UIComponent component, bool value)
        {
            bool updateAll = false;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.RightCommand))
                updateAll = true;
            ForestBrushes.instance.BrushTool.Update(treeInfo, value, updateAll);
        }

        public void Deselect(bool isRowOdd)
        {
            
        }

        public void Display(object data, bool isRowOdd)
        {
            treeInfo = data as TreeInfo;
            Initialize(treeInfo);
            includeCheckBox.isChecked = ForestBrushes.instance.BrushTool.Container.m_variations.Any(v => v.m_finalTree == treeInfo);
            treeNameLabel.text = treeInfo.GetUncheckedLocalizedTitle();
            thumbNailSprite.texture = treeInfo.m_Atlas.sprites.Find(spr => spr.name == treeInfo.m_Thumbnail).texture;
            backgroundSprite = null;
            if (isRowOdd)
                backgroundSprite = "ListItemHover";
        }

        public void Select(bool isRowOdd)
        {
            
        }
    }
}
