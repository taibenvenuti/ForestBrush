using ColossalFramework;
using ColossalFramework.UI;
using ForestBrush.Resources;
using ForestBrush.TranslationFramework;
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
        static NewBrushModal instance;
        internal static NewBrushModal Instance
        {
            get
            {
                if (instance == null)
                    instance = UIView.GetAView().AddUIComponent(typeof(NewBrushModal)) as NewBrushModal;
                return instance;
            }
        }

        public override void Start()
        {
            base.Start();
            size = new Vector2(Constants.UIPanelSize.x, 130f);
            relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
            backgroundSprite = "MenuPanel";
            isVisible = true;
            isInteractive = true;

            title = AddUIComponent<UILabel>();
            title.text = Translation.Instance.GetTranslation("FOREST-BRUSH-PROMPT-NEW");
            title.textScale = Constants.UITitleTextScale;
            title.relativePosition = new Vector3((width - title.width) / 2, (Constants.UITitleBarHeight - title.height) / 2);

            dragHandle = AddUIComponent<UIDragHandle>();
            dragHandle.size = new Vector2(Constants.UIPanelSize.x, Constants.UITitleBarHeight);
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = this;
            
            textField = AddUIComponent<UITextField>();
            textField.atlas =  ResourceLoader.GetAtlas("Ingame");
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
                if (!char.IsLetterOrDigit(e.character) && !char.IsWhiteSpace(e.character) && !char.IsControl(e.character)) e.Use();
                if (string.IsNullOrEmpty(textField.text))
                    okButton.Disable();
                else okButton.Enable();
            };

            okButton = UIUtilities.CreateButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-PROMPT-OK"));
            okButton.relativePosition = textField.relativePosition + new Vector3(0f, textField.height + Constants.UISpacing);
            okButton.width = (width - (Constants.UISpacing * 3)) / 2;
            okButton.eventClicked += (c, e) =>
            {
                ForestBrushMod.instance.BrushTool.New(textField.text);
                UIView.PopModal();
                Hide();
            };
            okButton.Disable();

            cancelButton = UIUtilities.CreateButton(this, Translation.Instance.GetTranslation("FOREST-BRUSH-PROMPT-CANCEL"));
            cancelButton.relativePosition = okButton.relativePosition + new Vector3(okButton.width + Constants.UISpacing, 0f);
            cancelButton.width = (width - (Constants.UISpacing * 3)) / 2;
            cancelButton.eventClicked += (c, e) =>
            {
                UIView.PopModal();
                Hide();
            };
        }

        protected override void OnKeyDown(UIKeyEventParameter p)
        {
            base.OnKeyDown(p);
            if (!p.used && p.keycode == KeyCode.Escape)
            {
                p.Use();
                cancelButton.SimulateClick();
            }

            if (!p.used && p.keycode == KeyCode.Return)
            {
                p.Use();
                okButton.SimulateClick();
            }
        }

        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();
            UIComponent modalEffect = GetUIView().panelsLibraryModalEffect;
            if (isVisible)
            {
                textField.text = string.Empty;
                textField.Focus();

                if (modalEffect != null)
                {
                    modalEffect.Show(false);
                    ValueAnimator.Animate("NewForestBrushModalEffect", (f) => modalEffect.opacity = f, new AnimatedFloat(0f, 1f, 0.7f, EasingType.CubicEaseOut));
                }
            }
            else if(modalEffect != null)
            {
                ValueAnimator.Animate("NewForestBrushModalEffect", (f) => modalEffect.opacity = f, new AnimatedFloat(1f, 0f, 0.7f, EasingType.CubicEaseOut), () => modalEffect.Hide());
            }
        }
    }
}
