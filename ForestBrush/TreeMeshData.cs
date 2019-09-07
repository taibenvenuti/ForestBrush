using System;
using UnityEngine;

namespace ForestBrush
{
    public class TreeMeshData
    {
        // Another class by SamSamTS
        private PrefabInfo m_prefab;

        public PrefabInfo prefab
        {
            get { return m_prefab; }
            set
            {
                if (m_prefab != value)
                {
                    m_prefab = value;

                    name = m_prefab.GetUncheckedLocalizedTitle();
                    textureSize = GetTextureSize(m_prefab);

                    GetTriangleInfo(m_prefab, out triangles);

                    steamID = GetSteamID(m_prefab);
                }
            }
        }

        public string name;
        public int triangles;
        public Vector2 textureSize;
        public string steamID;

        public static bool ascendingSort = true;

        public TreeMeshData(PrefabInfo prefab)
        {
            this.prefab = prefab;
        }

        private static string GetSteamID(PrefabInfo prefab)
        {
            string steamID = null;

            if (prefab.name.Contains("."))
            {
                steamID = prefab.name.Substring(0, prefab.name.IndexOf("."));
                if (!Int32.TryParse(steamID, out int id)) return null;
            }

            return steamID;
        }

        private static void GetTriangleInfo(PrefabInfo prefab, out int triangles)
        {
            Mesh mesh = null;
            triangles = 0;

            mesh = (prefab as TreeInfo).m_mesh;

            if (mesh != null && mesh.isReadable)
                triangles = mesh.triangles.Length / 3;
        }

        private static Vector2 GetTextureSize(PrefabInfo prefab)
        {
            Material material = null;
            if (prefab is BuildingInfo)
                material = (prefab as BuildingInfo).m_material;
            else if (prefab is PropInfo)
                material = (prefab as PropInfo).m_material;
            else if (prefab is TreeInfo)
                material = (prefab as TreeInfo).m_material;
            else if (prefab is VehicleInfo)
                material = (prefab as VehicleInfo).m_material;

            if (material != null && material.mainTexture != null)
                return new Vector2(material.mainTexture.width, material.mainTexture.height);

            return Vector2.zero;
        }
    }
}
