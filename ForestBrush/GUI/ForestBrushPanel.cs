using System;
using ColossalFramework.Globalization;
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

            LocaleManager.eventLocaleChanged += ForestBrushPanel_eventLocaleChanged;
            Hide();
        }

        public override void OnDestroy()
        {
            LocaleManager.eventLocaleChanged -= ForestBrushPanel_eventLocaleChanged;
            base.OnDestroy();
        }
        private void ForestBrushPanel_eventLocaleChanged()
        {
            BrushSelectSection.LocaleChanged();
            BrushEditSection.LocaleChanged();
            BrushOptionsSection.LocaleChanged();
        }

        private void Setup()
        {
            width = 400f;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoFitChildrenVertically = true;
            autoLayoutPadding = new RectOffset(0, 0, 0, 10);
            absolutePosition = new Vector3(UserMod.Settings.PanelPosX, UserMod.Settings.PanelPosY);
            atlas = ResourceLoader.Atlas;
            backgroundSprite = ResourceLoader.MenuPanel;
            isInteractive = true;
        }

        internal void LoadBrush(Brush brush)
        {
            BrushSelectSection?.LoadBrush(brush);
            BrushEditSection?.LoadBrush(brush);
            BrushOptionsSection?.LoadBrush(brush);
        }

        internal void KeepWithinScreen()
        {
            ClampToScreen();
            if (relativePosition.y + height > Screen.height - 130.0f) relativePosition -= new Vector3(0.0f, 130.0f, 0.0f);
        }
    }
}
