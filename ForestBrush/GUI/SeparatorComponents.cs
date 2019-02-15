using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    public class SeparatorComponents
    {
        public SeparatorComponents(GameObject mainToolbarSeparatorTemplate, GameObject emptyContainer, UIComponent separatorTab)
        {
            MainToolbarSeparatorTemplate = mainToolbarSeparatorTemplate;
            EmptyContainer = emptyContainer;
            SeparatorTab = separatorTab;
        }

        public GameObject MainToolbarSeparatorTemplate { get; }
        public GameObject EmptyContainer { get; }
        public UIComponent SeparatorTab { get; }
    }
}
