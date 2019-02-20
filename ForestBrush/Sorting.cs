
namespace ForestBrush
{
    public enum TreeSorting
    {
        Name,
        Author,
        Texture,
        Triangles
    }

    public static class SortingExtension
    {
        public static int CompareTo(this TreeInfo info, object obj, TreeSorting sorting, bool ascendingSort = false)
        {
            TreeInfo a, b;

            if (!ascendingSort)
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

            string authorB, authorA = string.Empty;
            TreeMeshData meshDataB, meshDataA = null;

            bool haveMeshData = ForestBrush.Instance.TreesMeshData.TryGetValue(b.name, out meshDataB) && ForestBrush.Instance.TreesMeshData.TryGetValue(a.name, out meshDataA);
            bool haveAuthors = ForestBrush.Instance.TreeAuthors.TryGetValue(b.name, out authorB) && ForestBrush.Instance.TreeAuthors.TryGetValue(a.name, out authorA);

            if (sorting == TreeSorting.Name)
                return b.GetUncheckedLocalizedTitle().CompareTo(a.GetUncheckedLocalizedTitle());
            if (sorting == TreeSorting.Author)
                if (haveAuthors)
                    return authorB.CompareTo(authorA);
            if (sorting == TreeSorting.Texture)
                if (haveMeshData)
                    return (meshDataB.textureSize.x + meshDataB.textureSize.y).CompareTo(meshDataA.textureSize.x + meshDataA.textureSize.y);
            if (sorting == TreeSorting.Triangles)
                if (haveMeshData)
                    return meshDataB.triangles.CompareTo(meshDataA.triangles);
            return 0;
        }
    }
}
