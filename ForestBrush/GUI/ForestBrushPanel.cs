using ColossalFramework.UI;
using ForestBrush.Resources;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class ForestBrushPanel : UIPanel
    {
        internal TitleSection TitleSection;
        internal BrushSelectSection BrushSelectSection;
        internal BrushEditSection BrushEditSection;
        internal BrushOptionsSection BrushOptionsSection;
        UIPanel layoutPanelSpace;

        internal bool ToggleBrushEdit()
        {
            BrushEditSection.isVisible = !BrushEditSection.isVisible;
            return BrushEditSection.isVisible;
        }

        internal bool ToggleBrushOptions()
        {
            BrushOptionsSection.isVisible = !BrushOptionsSection.isVisible;
            return BrushOptionsSection.isVisible;
        }

        public override void Start()
        {
            base.Start();
            Setup();
            TitleSection = AddUIComponent<TitleSection>();
            BrushSelectSection = AddUIComponent<BrushSelectSection>();
            BrushEditSection = AddUIComponent<BrushEditSection>();
            BrushOptionsSection = AddUIComponent<BrushOptionsSection>();
            layoutPanelSpace = AddUIComponent<UIPanel>();
            layoutPanelSpace.size = new Vector2(width, 1);

            TitleSection.zOrder = 0;
            BrushSelectSection.zOrder = 1;
            BrushEditSection.zOrder = 2;
            BrushOptionsSection.zOrder = 3;
            layoutPanelSpace.zOrder = 4;

            Hide();
        }

        private void Setup()
        {
            width = 400f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 0, 10);
            absolutePosition = new Vector3(ForestBrushMod.instance.Settings.PanelX, ForestBrushMod.instance.Settings.PanelY);
            atlas = ResourceLoader.GetAtlas("Ingame");
            backgroundSprite = "MenuPanel";
            isInteractive = true;
        }

        internal void LoadBrush(ForestBrush brush)
        {
            BrushSelectSection?.LoadBrush(brush);
            BrushEditSection?.LoadBrush(brush);
            BrushOptionsSection?.LoadBrush(brush);
        }
    }
}
