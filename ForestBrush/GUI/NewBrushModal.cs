using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class NewBrushModal : UIPanel
    {
        UIDragHandle dragHandle;
        UILabel title;
        UITextField textField;
        UIButton okButton;
        UIButton cancelButton;

        public override void Start()
        {
            base.Start();
            size = new Vector2(Constants.UIPanelSize.x, 130f);
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            backgroundSprite = "MenuPanel";
            isVisible = true;
            isInteractive = true;

            title = AddUIComponent<UILabel>();
            title.text = UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-NEW");
            title.textScale = Constants.UITextScale;
            title.relativePosition = new Vector3((width - title.width) / 2, (Constants.UITitleBarHeight - title.height) / 2);

            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(Constants.UIPanelSize.x, Constants.UITitleBarHeight);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;
            
            textField = AddUIComponent<UITextField>();
            textField.atlas = UIUtilities.GetAtlas();
            textField.size = new Vector2(width - Constants.UISpacing * 2, 30f);
            textField.padding = new RectOffset(6, 6, 6, 6);
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;
            textField.horizontalAlignment = UIHorizontalAlignment.Center;
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanelHovered";
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(80, 80, 80, 128);
            textField.color = new Color32(255, 255, 255, 255);
            textField.relativePosition = new Vector3(width - textField.width - Constants.UISpacing, Constants.UITitleBarHeight + Constants.UISpacing);
            textField.eventKeyPress += (c, e) =>
            {
                if (!char.IsLetterOrDigit(e.character) && !char.IsControl(e.character)) e.Use();
                if (string.IsNullOrEmpty(textField.text))
                    okButton.Disable();
                else okButton.Enable();
            };

            okButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-OK"));
            okButton.relativePosition = textField.relativePosition + new Vector3(0f, textField.height + Constants.UISpacing);
            okButton.width = (width - (Constants.UISpacing * 3)) / 2;
            okButton.eventClicked += (c, e) =>
            {
                ForestBrushMod.instance.BrushTool.New(textField.text);
                UIView.PopAllModal();
                DestroyImmediate(gameObject);
            };
            okButton.Disable();

            cancelButton = UIUtilities.CreateButton(this, UserMod.Translation.GetTranslation("FOREST-BRUSH-PROMPT-CANCEL"));
            cancelButton.relativePosition = textField.relativePosition + new Vector3(okButton.relativePosition.x + okButton.width + Constants.UISpacing, textField.height + Constants.UISpacing);
            cancelButton.width = (width - (Constants.UISpacing * 3)) / 2;
            cancelButton.eventClicked += (c, e) =>
            {
                UIView.PopAllModal();
                DestroyImmediate(gameObject);
            };
        }
    }
}
