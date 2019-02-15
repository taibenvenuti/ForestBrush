using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class ToggleButtonComponents
    {
        public ToggleButtonComponents(SeparatorComponents preSeparator, GameObject tabStripPage, GameObject mainToolbarButtonTemplate, UIButton toggleButton, SeparatorComponents postSeparator)
        {
            PreSeparatorComponents = preSeparator;
            ToggleButton = toggleButton;
            TabStripPage = tabStripPage;
            MainToolbarButtonTemplate = mainToolbarButtonTemplate;
            PostSeparatorComponents = postSeparator;
        }

        public SeparatorComponents PreSeparatorComponents { get; }
        public GameObject TabStripPage { get; }
        public GameObject MainToolbarButtonTemplate { get; }
        public UIButton ToggleButton { get; }
        public SeparatorComponents PostSeparatorComponents { get; }
    }
}
