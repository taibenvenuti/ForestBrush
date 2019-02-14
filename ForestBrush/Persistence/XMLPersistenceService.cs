using ColossalFramework;
using ColossalFramework.IO;
using System.IO;
using System.Linq;
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
                Brushes = settings.Brushes.Values.ToList(),
                SelectedBrush = settings.SelectedBrush.Name,
                Search = GetXmlInputKey(settings.Search),
                ToggleTool = GetXmlInputKey(settings.ToggleTool),
                ToggleSquare = GetXmlInputKey(settings.ToggleSquare),
                ToggleAutoDensity = GetXmlInputKey(settings.ToggleAutoDensity),
                IncreaseSize = GetXmlInputKey(settings.IncreaseSize),
                DecreaseSize = GetXmlInputKey(settings.DecreaseSize),
                IncreaseDensity = GetXmlInputKey(settings.IncreaseDensity),
                DecreaseDensity = GetXmlInputKey(settings.DecreaseDensity),
                IncreaseStrength = GetXmlInputKey(settings.IncreaseStrength),
                DecreaseStrength = GetXmlInputKey(settings.DecreaseStrength)
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
                    xmlSettings.Brushes,
                    xmlSettings.SelectedBrush,
                    GetSavedInputKey(xmlSettings.Search),
                    GetSavedInputKey(xmlSettings.ToggleTool),
                    GetSavedInputKey(xmlSettings.ToggleSquare),
                    GetSavedInputKey(xmlSettings.ToggleAutoDensity),
                    GetSavedInputKey(xmlSettings.IncreaseSize),
                    GetSavedInputKey(xmlSettings.DecreaseSize),
                    GetSavedInputKey(xmlSettings.IncreaseDensity),
                    GetSavedInputKey(xmlSettings.DecreaseDensity),
                    GetSavedInputKey(xmlSettings.IncreaseStrength),
                    GetSavedInputKey(xmlSettings.DecreaseStrength)
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
