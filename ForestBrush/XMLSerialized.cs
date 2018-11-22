using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ForestBrush
{
    [XmlRoot("ForestBrush")]
    public class XMLSerialized
    {
        [XmlIgnore]
        public static readonly string ConfigurationPath = Path.Combine(DataLocation.localApplicationData, "ForestBrush.xml");       

        public List<ForestBrush> SavedBrushes { get; set; } = new List<ForestBrush>() { new ForestBrush() };

        public OverlayColor OverlayColor { get; set; } = new Color32(25, 125, 155, 255);  

        public XMLSerialized() { }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { }

        public void Save()
        {   
            var fileName = ConfigurationPath;
            var serializer = new XmlSerializer(typeof(XMLSerialized));
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


        public static XMLSerialized Load()
        {
            var fileName = ConfigurationPath;
            var serializer = new XmlSerializer(typeof(XMLSerialized));
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    return serializer.Deserialize(reader) as XMLSerialized;                   
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Parsing {fileName}: {ex}");
                return new XMLSerialized();
            }
        }
    }
}
