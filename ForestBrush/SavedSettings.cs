using ColossalFramework;
using UnityEngine;

namespace ForestBrush
{
    public static class SavedSettings
    {
        public static string FileName = "ForestBrush";

        public static SavedString SelectedBrush = new SavedString("selectedBrush", FileName, Constants.VanillaPack, true);

        public static SavedFloat PanelX = new SavedFloat("panelX", FileName, 230f, true);

        public static SavedFloat PanelY = new SavedFloat("panelY", FileName, 0f, true);

        public static SavedFloat BrushDensity = new SavedFloat("brushDensity", FileName, 4f, true);

        public static SavedFloat BrushSize = new SavedFloat("brushSize", FileName, 15f, true);

        public static SavedBool ConfirmOverwrite = new SavedBool("confirmOverwrite", FileName, true, true);

        public static SavedBool AutoDensity = new SavedBool("autoDensity", FileName, true, true);

        public static SavedBool SquareBrush = new SavedBool("squareBrush", FileName, false, true);

        public static SavedInputKey Search = new SavedInputKey("search", FileName, SavedInputKey.Encode(KeyCode.F, false, false, true), true);

        public static SavedInputKey ToggleTool = new SavedInputKey("toggleTool", FileName, SavedInputKey.Encode(KeyCode.B, false, false, true), true);

        public static SavedInputKey ToggleSquare = new SavedInputKey("toggleSquare", FileName, SavedInputKey.Encode(KeyCode.S, false, false, true), true);

        public static SavedInputKey ToggleAutoDensity = new SavedInputKey("toggleAutoDensity", FileName, SavedInputKey.Encode(KeyCode.D, false, false, true), true);

        public static SavedInputKey IncreaseSize = new SavedInputKey("increaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha2, false, false, true), true);

        public static SavedInputKey DecreaseSize = new SavedInputKey("decreaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha1, false, false, true), true);

        public static SavedInputKey IncreaseDensity = new SavedInputKey("increaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha4, false, false, true), true);

        public static SavedInputKey DecreaseDensity = new SavedInputKey("decreaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha3, false, false, true), true);

        public static void Reset()
        {
            SelectedBrush = new SavedString("selectedBrush", FileName, Constants.VanillaPack, true);

            PanelX = new SavedFloat("panelX", FileName, -550f, true);

            PanelY = new SavedFloat("panelY", FileName, -800f, true);

            BrushDensity = new SavedFloat("brushDensity", FileName, 4f, true);

            BrushSize = new SavedFloat("brushSize", FileName, 15f, true);

            ConfirmOverwrite = new SavedBool("confirmOverwrite", FileName, true, true);

            AutoDensity = new SavedBool("autoDensity", FileName, true, true);

            SquareBrush = new SavedBool("squareBrush", FileName, false, true);

            Search = new SavedInputKey("search", FileName, SavedInputKey.Encode(KeyCode.S, false, false, true), true);

            ToggleTool = new SavedInputKey("toggle", FileName, SavedInputKey.Encode(KeyCode.B, false, false, true), true);

            ToggleSquare = new SavedInputKey("toggleSquare", FileName, SavedInputKey.Encode(KeyCode.S, false, false, true), true);

            ToggleAutoDensity = new SavedInputKey("toggleAutoDensity", FileName, SavedInputKey.Encode(KeyCode.D, false, false, true), true);

            IncreaseSize = new SavedInputKey("increaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha1, false, false, true), true);

            DecreaseSize = new SavedInputKey("decreaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha2, false, false, true), true);

            IncreaseDensity = new SavedInputKey("increaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha3, false, false, true), true);

            DecreaseDensity = new SavedInputKey("decreaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha4, false, false, true), true);

            GameSettings.SaveAll();

        }
    }
}
