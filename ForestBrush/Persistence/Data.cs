using System;
using System.Collections.Generic;

using ColossalFramework.IO;
using UnityEngine;

namespace ForestBrush.Persistence
{
    public class Data : IDataContainer
    {
        public class PrecisionData
        {
            public ushort x;
            public ushort z;
        }

        public static Dictionary<ushort, PrecisionData> data = new Dictionary<ushort, PrecisionData>();

        public void Serialize(DataSerializer s) {
            try {
                int count = 0;
                foreach (ushort tree in data.Keys) {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        count++;
                    }
                }

                s.WriteInt32(count);
                foreach (ushort tree in data.Keys) {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        s.WriteUInt16(tree);
                        s.WriteUInt16(data[tree].x);
                        s.WriteUInt16(data[tree].z);
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void Deserialize(DataSerializer s) {
            try {
                data.Clear();

                var arraySize = s.ReadInt32();

                for (int i = 0; i < arraySize; i++) {
                    ushort tree = (ushort)s.ReadUInt16();

                    PrecisionData value = new PrecisionData();
                    value.x = (ushort)s.ReadUInt16();
                    value.z = (ushort)s.ReadUInt16();

                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        data[tree] = value;
                    }
                }
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AfterDeserialize(DataSerializer s) {
        }
    }
}