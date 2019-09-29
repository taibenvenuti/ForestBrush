using ColossalFramework.IO;
using ICities;
using System.IO;
using System.Linq;

namespace ForestBrush.Persistence
{
    public class SerializableDataExtension : SerializableDataExtensionBase
    {
        private const string PRECISION_ID = "ForestBrush-TreePrecision";
        private const string GROWSTATE_ID = "ForestBrush-GrowState";
        private const int VERSION = 1;

        public override void OnLoadData() {
            base.OnLoadData();
            if (ToolManager.instance.m_properties.m_mode != ItemClass.Availability.Game) return;

            if (serializableDataManager.EnumerateData().Contains(GROWSTATE_ID)) {
                var growStateData = serializableDataManager.LoadData(GROWSTATE_ID);
                using (var ms = new MemoryStream(growStateData)) {
                    var s = DataSerializer.Deserialize<GrowState>(ms, DataSerializer.Mode.Memory);
                }
            }

            if (UserMod.IsModEnabled(1873351912UL, "Tree Precision")) return;
            if (!serializableDataManager.EnumerateData().Contains(PRECISION_ID)) return;
            var data = serializableDataManager.LoadData(PRECISION_ID);
            using (var ms = new MemoryStream(data)) {
                var s = DataSerializer.Deserialize<Precision>(ms, DataSerializer.Mode.Memory);
            }
        }

        public override void OnSaveData() {
            base.OnSaveData();
            if (ToolManager.instance.m_properties.m_mode != ItemClass.Availability.Game) return;

            using (var ms = new MemoryStream()) {
                DataSerializer.Serialize(ms, DataSerializer.Mode.Memory, VERSION, new GrowState());
                var data = ms.ToArray();
                serializableDataManager.SaveData(GROWSTATE_ID, data);
            }

            if (UserMod.IsModEnabled(1873351912UL, "Tree Precision")) return;
            using (var ms = new MemoryStream()) {
                DataSerializer.Serialize(ms, DataSerializer.Mode.Memory, VERSION, new Precision());
                var data = ms.ToArray();    
                serializableDataManager.SaveData(PRECISION_ID, data);
            }
        }
    }
}
