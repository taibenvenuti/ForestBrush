using ColossalFramework.IO;
using ICities;
using System.IO;
using System.Linq;

namespace ForestBrush.Persistence
{
    public class SerializableDataExtension : SerializableDataExtensionBase
    {
        private const string ID = "TreePrecision";
        private const int VERSION = 1;

        public override void OnLoadData() {
            base.OnLoadData();
            if (UserMod.IsModEnabled(1873351912UL, "Tree Precision")) return;
            if (ToolManager.instance.m_properties.m_mode != ItemClass.Availability.Game) {
                return;
            }
            if (!serializableDataManager.EnumerateData().Contains(ID)) {
                return;
            }
            var data = serializableDataManager.LoadData(ID);
            using (var ms = new MemoryStream(data)) {
                var s = DataSerializer.Deserialize<Data>(ms, DataSerializer.Mode.Memory);
            }
        }

        public override void OnSaveData() {
            base.OnSaveData();
            if (UserMod.IsModEnabled(1873351912UL, "Tree Precision")) return;
            if (ToolManager.instance.m_properties.m_mode != ItemClass.Availability.Game) {
                return;
            }
            using (var ms = new MemoryStream()) {
                DataSerializer.Serialize(ms, DataSerializer.Mode.Memory, VERSION, new Data());
                var data = ms.ToArray();
                serializableDataManager.SaveData(ID, data);
            }
        }
    }
}
