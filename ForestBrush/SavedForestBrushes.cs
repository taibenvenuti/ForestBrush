using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace ForestBrush
{
    [XmlRoot("SavedForestBrushes")]
    public class SavedForestBrushes
    {
        [XmlIgnore]
        public static readonly string ConfigurationPath = Path.Combine(DataLocation.localApplicationData, "SavedForestBrushes.xml");

        public List<KeyValuePair<string, List<string>>> SavedBrushes { get; set; } = new List<KeyValuePair<string, List<string>>>()
        {
            new KeyValuePair<string, List<string>>(Constants.VanillaPack, new List<string>())
        };

        public OverlayColor OverlayColor { get; set; } = new Color32(25, 125, 155, 255);  

        public SavedForestBrushes() { }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { }

        public void Save()
        {
            SavedBrushes?.Clear();
            if (ForestBrushMod.instance.Brushes != null)
            {
                foreach (var brush in ForestBrushMod.instance.Brushes.ToList())
                    if (brush.Value != null)
                        SavedBrushes.Add(brush);
            }           
            
            var fileName = ConfigurationPath;
            var serializer = new XmlSerializer(typeof(SavedForestBrushes));
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    OnPreSerialize();
                    serializer.Serialize(writer, this);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Saving {fileName}: {ex}");
            }            
        }


        public static SavedForestBrushes Load()
        {
            var fileName = ConfigurationPath;
            var serializer = new XmlSerializer(typeof(SavedForestBrushes));
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var config = serializer.Deserialize(reader) as SavedForestBrushes;
                    ForestBrushMod.instance.Brushes.Clear();
                    ForestBrushMod.instance.Brushes = config.SavedBrushes?.ToDictionary(kv => kv.Key, kv => kv.Value);
                    ForestBrushMod.instance.ForestBrushPanel.UpdateDropDown();
                    return config;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Parsing {fileName}: {ex}");
                return new SavedForestBrushes();
            }
        }
    }
}
