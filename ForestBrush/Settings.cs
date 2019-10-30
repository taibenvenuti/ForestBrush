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
            bool brushShapesOpen,
            bool brushEditOpen,
            bool brushOptionsOpen,
            bool showTreeMeshData,
            TreeSorting sorting,
            SortingOrder sortingOrder,
            FilterStyle filterStyle,
            IEnumerable<Brush> forestBrushes,
            string selectedBrush,
            SavedInputKey toggleTool,
            bool keepTreesInNewBrush,
            bool ignoreVanillaTrees,
            bool showInfoTooltip,
            bool playEffect,
            bool chargeMoney
        )
        {
            this.PanelPosX = panelPosX;
            this.PanelPosY = panelPosY;
            this.BrushShapesOpen = brushShapesOpen;
            this.BrushEditOpen = brushEditOpen;
            this.BrushOptionsOpen = brushOptionsOpen;
            this.ShowTreeMeshData = showTreeMeshData;
            this.Sorting = sorting;
            this.SortingOrder = sortingOrder;
            this.FilterStyle = filterStyle;
            this.ToggleTool = toggleTool;
            this.KeepTreesInNewBrush = keepTreesInNewBrush;
            this.IgnoreVanillaTrees = ignoreVanillaTrees;
            this.ShowInfoTooltip = showInfoTooltip;
            this.PlayEffect = playEffect;
            this.ChargeMoney = chargeMoney;
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

        public bool BrushShapesOpen { get; set; }

        public bool BrushEditOpen { get; set; }

        public bool BrushOptionsOpen { get; set; }

        public bool ShowTreeMeshData { get; set; }

        public TreeSorting Sorting { get; set; }

        public SortingOrder SortingOrder { get; set; }

        public FilterStyle FilterStyle { get; set; }

        public Brush SelectedBrush { get; private set; }

        public SavedInputKey ToggleTool { get; set; }

        public bool KeepTreesInNewBrush { get; internal set; }

        public bool IgnoreVanillaTrees { get; internal set; }

        public bool ShowInfoTooltip { get; internal set; }

        public bool PlayEffect { get; internal set; }

        public bool ChargeMoney { get; internal set; }

        public List<Brush> Brushes { get; set; }

        public static Settings Default()
        {
            var defaultBrush = Brush.Default();

            return new Settings(
                200f,
                100f,
                false,
                false,
                false,
                true,
                TreeSorting.Name,
                SortingOrder.Ascending,
                FilterStyle.AND,
                Enumerable.Empty<Brush>(),
                string.Empty,
                new SavedInputKey("toggleTool", Constants.ModName, SavedInputKey.Encode(KeyCode.B, false, false, true), true),
                false,
                false,
                true,
                true,
                true
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
    }
}
