using System;
using System.Collections.Generic;
using ColossalFramework.IO;
using UnityEngine;

namespace ForestBrush.Persistence
{
    public class GrowState : IDataContainer
    {

        public static HashSet<uint> data = new HashSet<uint>();

        public void Serialize(DataSerializer s) {
            int count = 0;
            foreach (uint tree in data) {
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
            foreach (uint tree in data) {
                try {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        s.WriteUInt32(tree);
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
                try {
                    if ((TreeManager.instance.m_trees.m_buffer[tree].m_flags & (ushort)TreeInstance.Flags.Created) == (ushort)TreeInstance.Flags.Created) {
                        data.Add(tree);
                    }
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
        }

        public void AfterDeserialize(DataSerializer s) {
            foreach (uint tree in data) {
                try {
                    TreeManager.instance.m_trees.m_buffer[tree].GrowState = 1;
                } catch (Exception e) {
                    Debug.LogException(e);
                    continue;
                }
            }
        }
    }
}