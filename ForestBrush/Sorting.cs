
using UnityEngine;

namespace ForestBrush
{
    public enum TreeSorting
    {
        Name,
        Author,
        Texture,
        Triangles
    }

    public enum SortingOrder
    {
        Descending,
        Ascending
    }

    public static class SortingExtension
    {
        public static int CompareTo(this TreeInfo info, object obj, TreeSorting sorting, SortingOrder order)
        {
            TreeInfo a, b;

            if (order == SortingOrder.Descending)
            {
                a = info;
                b = obj as TreeInfo;
            }
            else
            {
                b = info;
                a = obj as TreeInfo;
            }
            if (a == null || b == null) return -1;

            if (sorting == TreeSorting.Name)
                return b.GetUncheckedLocalizedTitle().CompareTo(a.GetUncheckedLocalizedTitle());

            if (sorting == TreeSorting.Author) {
                if (!ForestBrush.Instance.TreeAuthors.TryGetValue(a.name.Split('.')[0], out string authorA)) {
                    authorA = " ";
                }
                if (!ForestBrush.Instance.TreeAuthors.TryGetValue(b.name.Split('.')[0], out string authorB)) {
                    authorB = " ";
                }
                return authorB.CompareTo(authorA);
            }

            TreeMeshData meshDataB, meshDataA = null;
            bool aHasMeshData = ForestBrush.Instance.TreesMeshData.TryGetValue(a.name, out meshDataA);
            bool bHasMeshData = ForestBrush.Instance.TreesMeshData.TryGetValue(b.name, out meshDataB);
            bool haveMeshData = aHasMeshData && bHasMeshData;

            if (sorting == TreeSorting.Texture) {
                if (haveMeshData)
                    return (meshDataB.textureSize.x + meshDataB.textureSize.y).CompareTo(meshDataA.textureSize.x + meshDataA.textureSize.y);
                else if (aHasMeshData)
                    return 0.CompareTo(meshDataA.textureSize.x + meshDataA.textureSize.y);
                else if (bHasMeshData) {
                    return (meshDataB.textureSize.x + meshDataB.textureSize.y).CompareTo(0);
                }
            }

            if (sorting == TreeSorting.Triangles) {
                if (haveMeshData)
                    return meshDataB.triangles.CompareTo(meshDataA.triangles);
                else if (aHasMeshData)
                    return 0.CompareTo(meshDataA.triangles);
                else if (bHasMeshData) {
                    return meshDataB.triangles.CompareTo(0);
                }
            }

            return -1;
        }
    }
}
