using ColossalFramework;
using ForestBrush.Redirection;
using UnityEngine;

namespace ForestBrush
{
    [TargetType(typeof(TreeInstance))]
    public struct PositionDetour
    {
        [RedirectMethod]
        public unsafe Vector3 Position {
            get {
                fixed (void* pointer = &this) {
                    TreeInstance* tree = (TreeInstance*)pointer;

                    if (Singleton<ToolManager>.instance.m_properties.m_mode == ItemClass.Availability.AssetEditor) {
                        Vector3 result;
                        result.x = (float)tree->m_posX * 0.0164794922f;
                        result.y = (float)tree->m_posY * 0.015625f;
                        result.z = (float)tree->m_posZ * 0.0164794922f;
                        return result;
                    } else {
                        Vector3 result;

                        uint index;
                        fixed (TreeInstance* buffer = TreeManager.instance.m_trees.m_buffer) {
                            index = (uint)(tree - buffer);
                        }

                        if (Persistence.Precision.data.ContainsKey(index)) {
                            Persistence.Precision.Data precisionData = (Persistence.Precision.Data)Persistence.Precision.data[index];

                            if (tree->m_posX > 0) {
                                result.x = ((float)tree->m_posX + (float)precisionData.x / (float)ushort.MaxValue) * 0.263671875f;
                            } else {
                                result.x = ((float)tree->m_posX - (float)precisionData.x / (float)ushort.MaxValue) * 0.263671875f;
                            }

                            if (tree->m_posZ > 0) {
                                result.z = ((float)tree->m_posZ + (float)precisionData.z / (float)ushort.MaxValue) * 0.263671875f;
                            } else {
                                result.z = ((float)tree->m_posZ - (float)precisionData.z / (float)ushort.MaxValue) * 0.263671875f;
                            }
                            result.y = (float)tree->m_posY * 0.015625f;
                        } else {
                            result.x = (float)tree->m_posX * 0.263671875f;
                            result.y = (float)tree->m_posY * 0.015625f;
                            result.z = (float)tree->m_posZ * 0.263671875f;
                        }
                        return result;
                    }
                }
            }
            set {
                fixed (void* pointer = &this) {
                    TreeInstance* tree = (TreeInstance*)pointer;

                    if (Singleton<ToolManager>.instance.m_properties.m_mode == ItemClass.Availability.AssetEditor) {
                        tree->m_posX = (short)Mathf.Clamp(Mathf.RoundToInt(value.x * 60.68148f), -32767, 32767);
                        tree->m_posZ = (short)Mathf.Clamp(Mathf.RoundToInt(value.z * 60.68148f), -32767, 32767);
                        tree->m_posY = (ushort)Mathf.Clamp(Mathf.RoundToInt(value.y * 64f), 0, 65535);
                    } else {
                        tree->m_posX = (short)Mathf.Clamp((int)(value.x * 3.79259253f), -32767, 32767);
                        tree->m_posZ = (short)Mathf.Clamp((int)(value.z * 3.79259253f), -32767, 32767);
                        tree->m_posY = (ushort)Mathf.Clamp(Mathf.RoundToInt(value.y * 64f), 0, 65535);
                        Persistence.Precision.Data precisionData = new Persistence.Precision.Data();
                        precisionData.x = (ushort)(ushort.MaxValue * Mathf.Abs(value.x * 3.79259253f - (float)tree->m_posX));
                        precisionData.z = (ushort)(ushort.MaxValue * Mathf.Abs(value.z * 3.79259253f - (float)tree->m_posZ));

                        fixed (TreeInstance* buffer = TreeManager.instance.m_trees.m_buffer) {
                            Persistence.Precision.data[(uint)(tree - buffer)] = precisionData;
                        }
                    }
                }
            }
        }
    }
}