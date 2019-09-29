using System;
using System.Collections.Generic;
using ColossalFramework.IO;
using UnityEngine;

namespace ForestBrush.Persistence
{
    public class Precision : IDataContainer
    {
        public class Data
        {
            public ushort x;
            public ushort z;
        }

        public static Dictionary<uint, Data> data = new Dictionary<uint, Data>();

        public void Serialize(DataSerializer s) {
            int count = 0;
            foreach (uint tree in data.Keys) {
                try {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        count++;
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
            s.WriteInt32(count);
            foreach (uint tree in data.Keys) {
                try {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        s.WriteUInt32(tree);
                        s.WriteUInt16(data[tree].x);
                        s.WriteUInt16(data[tree].z);
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
        }

        public void Deserialize(DataSerializer s) {
            data.Clear();
            var arraySize = s.ReadInt32();
            for (int i = 0; i < arraySize; i++) {
                uint tree = (uint)s.ReadUInt32();
                Data value = new Data();
                value.x = (ushort)s.ReadUInt16();
                value.z = (ushort)s.ReadUInt16();
                try {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        data[tree] = value;
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
        }

        public void AfterDeserialize(DataSerializer s) {
        }
    }
}