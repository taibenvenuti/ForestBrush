using ColossalFramework;
using UnityEngine;

namespace ForestBrush
{
    public static class CGSSerialized
    {
        public static string FileName = "ForestBrush";

        public static SavedString SelectedBrush = new SavedString("selectedBrush", FileName, Constants.VanillaPack, true);

        public static SavedFloat PanelX = new SavedFloat("panelX", FileName, 1f, true);

        public static SavedFloat PanelY = new SavedFloat("panelY", FileName, -800f, true);

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
            GameSettings.FindSettingsFileByName(FileName).Delete();
        }
    }
}
