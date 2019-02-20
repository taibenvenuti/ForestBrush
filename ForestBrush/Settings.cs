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
            bool addRemoveTreesToBrushOpen,
            bool BrushOptionsOpen,
            bool ShowTreeMeshData,
            TreeSorting Sorting,
            IEnumerable<Brush> forestBrushes,
            string selectedBrush,
            SavedInputKey toggleTool
        )
        {
            this.PanelPosX = panelPosX;
            this.PanelPosY = panelPosY;
            this.AddRemoveTreesToBrushOpen = addRemoveTreesToBrushOpen;
            this.BrushOptionsOpen = BrushOptionsOpen;
            this.ShowTreeMeshData = ShowTreeMeshData;
            this.Sorting = Sorting;
            this.ToggleTool = toggleTool;

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

        public bool AddRemoveTreesToBrushOpen { get; set; }

        public bool BrushOptionsOpen { get; set; }

        public bool ShowTreeMeshData { get; set; }

        public TreeSorting Sorting { get; set; }

        public List<Brush> Brushes { get; set; }

        public Brush SelectedBrush { get; private set; }

        public SavedInputKey ToggleTool { get; set; }

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
                Enumerable.Empty<Brush>(),
                string.Empty,
                new SavedInputKey("toggleTool", Constants.ModName, SavedInputKey.Encode(KeyCode.B, false, false, true), true)
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
            this.AddRemoveTreesToBrushOpen = defaultSettings.AddRemoveTreesToBrushOpen;
            this.BrushOptionsOpen = defaultSettings.BrushOptionsOpen;
            this.ToggleTool = defaultSettings.ToggleTool;
            this.ShowTreeMeshData = defaultSettings.ShowTreeMeshData;
            this.Sorting = defaultSettings.Sorting;
        }
    }
}
