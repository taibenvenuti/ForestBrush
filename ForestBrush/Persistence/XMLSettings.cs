using System.Collections.Generic;
using System.Xml.Serialization;

namespace ForestBrush.Persistence
{
    [XmlRoot("ForestBrush")]
    public class XmlSettings {
        public XmlSettings() { }

        public float PanelPosX { get; set; }

        public float PanelPosY { get; set; }

        public bool BrushShapesOpen { get; set; }

        public bool BrushEditOpen { get; set; }

        public bool BrushOptionsOpen { get; set; }

        public bool ShowTreeMeshData { get; set; }

        public TreeSorting Sorting { get; set; }

        public SortingOrder SortingOrder { get; set; }

        public FilterStyle FilterStyle { get; set; }

        public string SelectedBrush { get; set; }

        public bool KeepTreesInNewBrush { get; set; }

        public bool IgnoreVanillaTrees { get; set; }

        public bool ShowInfoTooltip { get; internal set; }

        public bool PlayEffect { get; internal set; } = true;

        public bool ChargeMoney { get; internal set; } = true;

        public XmlInputKey Search { get; set; }

        public XmlInputKey ToggleTool { get; set; }

        public XmlInputKey ToggleSquare { get; set; }

        public XmlInputKey ToggleAutoDensity { get; set; }

        public XmlInputKey IncreaseSize { get; set; }

        public XmlInputKey DecreaseSize { get; set; }

        public XmlInputKey IncreaseDensity { get; set; }

        public XmlInputKey DecreaseDensity { get; set; }

        public XmlInputKey IncreaseStrength { get; set; }

        public XmlInputKey DecreaseStrength { get; set; }

        public List<Brush> Brushes { get; set; }
    }
}
