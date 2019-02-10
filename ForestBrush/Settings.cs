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
            IEnumerable<ForestBrush> forestBrushes,
            string selectedBrush,
            SavedInputKey search,
            SavedInputKey toggleTool,
            SavedInputKey toggleSquare,
            SavedInputKey toggleAutoDensity,
            SavedInputKey increaseSize,
            SavedInputKey decreaseSize,
            SavedInputKey increaseDensity,
            SavedInputKey decreaseDensity,
            SavedInputKey increaseStrength,
            SavedInputKey decreaseStrength
        )
        {
            this.PanelPosX = panelPosX;
            this.PanelPosY = panelPosY;
            this.AddRemoveTreesToBrushOpen = addRemoveTreesToBrushOpen;
            this.BrushOptionsOpen = BrushOptionsOpen;
            this.Search = search;
            this.ToggleTool = toggleTool;
            this.ToggleSquare = toggleSquare;
            this.ToggleAutoDensity = toggleAutoDensity;
            this.IncreaseSize = increaseSize;
            this.DecreaseSize = decreaseSize;
            this.IncreaseDensity = increaseDensity;
            this.DecreaseDensity = decreaseDensity;
            this.IncreaseStrength = increaseStrength;
            this.DecreaseStrength = decreaseStrength;

            this.Brushes = forestBrushes.ToDictionary(x => x.Name, x => x);
            if (this.Brushes.Count == 0)
            {
                var defaultBrush = ForestBrush.Default();
                this.Brushes.Add(defaultBrush.Name, defaultBrush);
            }

            this.SelectBrush(selectedBrush);
        }

        public float PanelPosX { get; set; }

        public float PanelPosY { get; set; }

        public bool AddRemoveTreesToBrushOpen { get; set; }

        public bool BrushOptionsOpen { get; set; }

        public Dictionary<string, ForestBrush> Brushes { get; set; }

        public ForestBrush SelectedBrush { get; private set; }

        public SavedInputKey Search { get; set; }

        public SavedInputKey ToggleTool { get; set; }

        public SavedInputKey ToggleSquare { get; set; }

        public SavedInputKey ToggleAutoDensity { get; set; }

        public SavedInputKey IncreaseSize { get; set; }

        public SavedInputKey DecreaseSize { get; set; }

        public SavedInputKey IncreaseDensity { get; set; }

        public SavedInputKey DecreaseDensity { get; set; }

        public SavedInputKey IncreaseStrength { get; set; }

        public SavedInputKey DecreaseStrength { get; set; }

        public static Settings Default()
        {
            var defaultBrush = ForestBrush.Default();

            return new Settings(
                200f,
                100f,
                false,
                false,
                Enumerable.Empty<ForestBrush>(),
                string.Empty,
                new SavedInputKey("search", Constants.ModName, SavedInputKey.Encode(KeyCode.F, false, false, true), true),
                new SavedInputKey("toggleTool", Constants.ModName, SavedInputKey.Encode(KeyCode.B, false, false, true), true),
                new SavedInputKey("toggleSquare", Constants.ModName, SavedInputKey.Encode(KeyCode.S, false, false, true), true),
                new SavedInputKey("toggleAutoDensity", Constants.ModName, SavedInputKey.Encode(KeyCode.D, false, false, true), true),
                new SavedInputKey("increaseSize", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha2, false, false, true), true),
                new SavedInputKey("decreaseSize", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha1, false, false, true), true),
                new SavedInputKey("increaseDensity", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha4, false, false, true), true),
                new SavedInputKey("decreaseDensity", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha3, false, false, true), true),
                new SavedInputKey("increaseStrength", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha6, false, false, true), true),
                new SavedInputKey("decreaseStrength", Constants.ModName, SavedInputKey.Encode(KeyCode.Alpha5, false, false, true), true)
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

        public void Reset()
        {
            var defaultSettings = Default();

            this.PanelPosX = defaultSettings.PanelPosX;
            this.PanelPosY = defaultSettings.PanelPosY;
            this.AddRemoveTreesToBrushOpen = defaultSettings.AddRemoveTreesToBrushOpen;
            this.BrushOptionsOpen = defaultSettings.BrushOptionsOpen;
            this.Brushes = defaultSettings.Brushes;
            this.SelectedBrush = defaultSettings.SelectedBrush;
            this.Search = defaultSettings.Search;
            this.ToggleTool = defaultSettings.ToggleTool;
            this.ToggleSquare = defaultSettings.ToggleSquare;
            this.ToggleAutoDensity = defaultSettings.ToggleAutoDensity;
            this.IncreaseSize = defaultSettings.IncreaseSize;
            this.DecreaseSize = defaultSettings.DecreaseSize;
            this.IncreaseDensity = defaultSettings.IncreaseDensity;
            this.DecreaseDensity = defaultSettings.DecreaseDensity;
            this.IncreaseStrength = defaultSettings.IncreaseStrength;
            this.DecreaseStrength = defaultSettings.DecreaseStrength;
        }

        private ForestBrush GetSelectedBrush(string brushName)
        {
            var found = Brushes.TryGetValue(brushName, out ForestBrush forestBrush);
            if (found)
            {
                return forestBrush;
            }
            else
            {
                return GetNextBestBrush();
            }
        }

        private ForestBrush GetNextBestBrush()
        {
            var first = Brushes.Values.FirstOrDefault();
            if (first == null)
            {
                var newDefault = ForestBrush.Default();
                Brushes.Add(newDefault.Name, newDefault);
                return newDefault;
            }
            else
            {
                return first;
            }
        }
    }
}
