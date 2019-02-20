using ColossalFramework;
using ColossalFramework.IO;
using System.IO;
using System.Xml.Serialization;

namespace ForestBrush.Persistence
{
    public class XmlPersistenceService
    {
        private readonly string configurationPath = Path.Combine(DataLocation.localApplicationData, "ForestBrush.xml");
        private readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlSettings));

        public void Save(Settings settings)
        {
            var xmlSettings = new XmlSettings()
            {
                PanelPosX = settings.PanelPosX,
                PanelPosY = settings.PanelPosY,
                AddRemoveTreesToBrushOpen = settings.AddRemoveTreesToBrushOpen,
                BrushOptionsOpen = settings.BrushOptionsOpen,
                ShowTreeMeshData = settings.ShowTreeMeshData,
                Sorting = settings.Sorting,
                Brushes = settings.Brushes,
                SelectedBrush = settings.SelectedBrush.Name,
                ToggleTool = GetXmlInputKey(settings.ToggleTool)
            };

            using (var sw = new StreamWriter(configurationPath))
            {
                xmlSerializer.Serialize(sw, xmlSettings);
            }
        }

        public Settings Load()
        {
            if (File.Exists(configurationPath))
            {
                XmlSettings xmlSettings;

                using (var sr = new StreamReader(configurationPath))
                {
                    xmlSettings = (XmlSettings)xmlSerializer.Deserialize(sr);
                }

                return new Settings(
                    xmlSettings.PanelPosX,
                    xmlSettings.PanelPosY,
                    xmlSettings.AddRemoveTreesToBrushOpen,
                    xmlSettings.BrushOptionsOpen,
                    xmlSettings.ShowTreeMeshData,
                    xmlSettings.Sorting,
                    xmlSettings.Brushes,
                    xmlSettings.SelectedBrush,
                    GetSavedInputKey(xmlSettings.ToggleTool)
                );
            }
            else
            {
                return Settings.Default();
            }
        }

        private XmlInputKey GetXmlInputKey(SavedInputKey savedInputKey)
        {
            return new XmlInputKey()
            {
                Name = savedInputKey.name,
                Key = savedInputKey.Key,
                Control = savedInputKey.Control,
                Alt = savedInputKey.Alt,
                Shift = savedInputKey.Shift,
            };
        }

        private SavedInputKey GetSavedInputKey(XmlInputKey xmlInputKey)
        {
            return new SavedInputKey(xmlInputKey.Name, Constants.ModName, xmlInputKey.Key, xmlInputKey.Control, xmlInputKey.Shift, xmlInputKey.Alt, true);
        }
    }
}
