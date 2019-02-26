using ColossalFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush
{
    public class Settings
    {
        public Settings(
            float panelPosX,
            float panelPosY,
            bool BrushEditOpen,
            bool BrushOptionsOpen,
            bool ShowTreeMeshData,
            TreeSorting Sorting,
            SortingOrder SortingOrder,
            FilterStyle FilterStyle,
            IEnumerable<Brush> forestBrushes,
            string selectedBrush,
            SavedInputKey toggleTool,
            bool keepTreesInNewBrush,
            bool ignoreVanillaTrees
        )
        {
            this.PanelPosX = panelPosX;
            this.PanelPosY = panelPosY;
            this.BrushEditOpen = BrushEditOpen;
            this.BrushOptionsOpen = BrushOptionsOpen;
            this.ShowTreeMeshData = ShowTreeMeshData;
            this.Sorting = Sorting;
            this.SortingOrder = SortingOrder;
            this.FilterStyle = FilterStyle;
            this.ToggleTool = toggleTool;
            this.KeepTreesInNewBrush = keepTreesInNewBrush;
            this.IgnoreVanillaTrees = ignoreVanillaTrees;

            this.Brushes = forestBrushes.ToList();
            if (this.Brushes.Count == 0)
            {
                var defaultBrush = Brush.Default();
                this.Brushes.Add(defaultBrush);
            }

            this.SelectBrush(selectedBrush);
        }

        public float PanelPosX { get; set; }

        public float PanelPosY { get; set; }

        public bool BrushEditOpen { get; set; }

        public bool BrushOptionsOpen { get; set; }

        public bool ShowTreeMeshData { get; set; }

        public TreeSorting Sorting { get; set; }

        public SortingOrder SortingOrder { get; set; }

        public FilterStyle FilterStyle { get; set; }

        public List<Brush> Brushes { get; set; }

        public Brush SelectedBrush { get; private set; }

        public SavedInputKey ToggleTool { get; set; }

        public bool KeepTreesInNewBrush { get; internal set; }

        public bool IgnoreVanillaTrees { get; internal set; }

        public static Settings Default()
        {
            var defaultBrush = Brush.Default();

            return new Settings(
                200f,
                100f,
                false,
                false,
                true,
                TreeSorting.Name,
                SortingOrder.Descending,
                FilterStyle.AND,
                Enumerable.Empty<Brush>(),
                string.Empty,
                new SavedInputKey("toggleTool", Constants.ModName, SavedInputKey.Encode(KeyCode.B, false, false, true), true),
                false,
                false
            );
        }

        public void SelectBrush(string brushName)
        {
            this.SelectedBrush = GetSelectedBrush(brushName);
        }

        public void SelectNextBestBrush()
        {
            this.SelectedBrush = GetNextBestBrush();
        }

        private Brush GetSelectedBrush(string brushName)
        {
            var brush = Brushes.Find(b => b.Name == brushName);
            if (brush != null)
            {
                return brush;
            }
            else
            {
                return GetNextBestBrush();
            }
        }

        private Brush GetNextBestBrush()
        {
            var first = Brushes.FirstOrDefault();
            if (first == null)
            {
                var newDefault = Brush.Default();
                Brushes.Add(newDefault);
                return newDefault;
            }
            else
            {
                return first;
            }
        }

        public void Reset()
        {
            var defaultSettings = Default();

            this.PanelPosX = defaultSettings.PanelPosX;
            this.PanelPosY = defaultSettings.PanelPosY;
            this.BrushEditOpen = defaultSettings.BrushEditOpen;
            this.BrushOptionsOpen = defaultSettings.BrushOptionsOpen;
            this.ToggleTool = defaultSettings.ToggleTool;
            this.ShowTreeMeshData = defaultSettings.ShowTreeMeshData;
            this.Sorting = defaultSettings.Sorting;
            this.SortingOrder = defaultSettings.SortingOrder;
            this.FilterStyle = defaultSettings.FilterStyle;
        }
    }
}
