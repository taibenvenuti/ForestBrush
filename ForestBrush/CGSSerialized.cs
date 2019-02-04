using ColossalFramework;
using UnityEngine;

namespace ForestBrush
{
    public class CGSSerialized
    {
        public static readonly string FileName = "ForestBrush";

        public SavedString SelectedBrush = new SavedString("selectedBrush", FileName, Constants.NewBrushName, true);

        public SavedFloat PanelX = new SavedFloat("panelX", FileName, 200f, true);

        public SavedFloat PanelY = new SavedFloat("panelY", FileName, 100f, true);

        public SavedFloat BrushStrength = new SavedFloat("brushStrength", FileName, 1f, true);

        public SavedFloat BrushDensity = new SavedFloat("brushDensity", FileName, 4f, true);

        public SavedFloat BrushSize = new SavedFloat("brushSize", FileName, 20f, true);

        public SavedBool AutoDensity = new SavedBool("autoDensity", FileName, true, true);

        public SavedBool SquareBrush = new SavedBool("squareBrush", FileName, false, true);

        public SavedInputKey Search = new SavedInputKey("search", FileName, SavedInputKey.Encode(KeyCode.F, false, false, true), true);

        public SavedInputKey ToggleTool = new SavedInputKey("toggleTool", FileName, SavedInputKey.Encode(KeyCode.B, false, false, true), true);

        public SavedInputKey ToggleSquare = new SavedInputKey("toggleSquare", FileName, SavedInputKey.Encode(KeyCode.S, false, false, true), true);

        public SavedInputKey ToggleAutoDensity = new SavedInputKey("toggleAutoDensity", FileName, SavedInputKey.Encode(KeyCode.D, false, false, true), true);

        public SavedInputKey IncreaseSize = new SavedInputKey("increaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha2, false, false, true), true);

        public SavedInputKey DecreaseSize = new SavedInputKey("decreaseSize", FileName, SavedInputKey.Encode(KeyCode.Alpha1, false, false, true), true);

        public SavedInputKey IncreaseDensity = new SavedInputKey("increaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha4, false, false, true), true);

        public SavedInputKey DecreaseDensity = new SavedInputKey("decreaseDensity", FileName, SavedInputKey.Encode(KeyCode.Alpha3, false, false, true), true);

        public SavedInputKey IncreaseStrength = new SavedInputKey("increaseStrength", FileName, SavedInputKey.Encode(KeyCode.Alpha6, false, false, true), true);

        public SavedInputKey DecreaseStrength = new SavedInputKey("decreaseStrength", FileName, SavedInputKey.Encode(KeyCode.Alpha5, false, false, true), true);

        public void Reset()
        {
            GameSettings.FindSettingsFileByName(FileName).Delete();
        }
    }
}
