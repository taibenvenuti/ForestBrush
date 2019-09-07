using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    public enum Filter
    {
        None = -1,
        Texture,
        Tris,
        InBrush,
        NotInBrush,
        Author,
        String,
        Count
    }

    public enum FilterStyle
    {
        AND,
        OR,
        Basic
    }

    public class Filtering
    {
        List<TreeInfo>[] filteredData;
        List<TreeInfo>[] FilteredData
        {
            get
            {
                if (filteredData == null)
                {
                    filteredData = new List<TreeInfo>[(int)Filter.Count + 1];
                    for (int i = 0; i < filteredData.Length; i++)
                    {
                        filteredData[i] = new List<TreeInfo>();
                    }
                }
                return filteredData;
            }
        }

        List<TreeInfo> _data;
        List<TreeInfo> Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new List<TreeInfo>();
                }
                return _data;
            }
        }

        bool[] _hasMatch;
        bool[] hasMatch
        {
            get
            {
                if (_hasMatch == null)
                {
                    _hasMatch = new bool[(int)Filter.Count];
                }
                return _hasMatch;
            }
        }

        bool IsMatch(Filter filter, TreeInfo tree)
        {
            bool isMatch = true;
            for (int i = 0; i < (int)Filter.Count; i++)
            {
                for (int j = 0; j < (int)Filter.Count; j++)
                {
                    if (hasMatch[i] && FilteredData[i].Contains(tree) && hasMatch[j] && !FilteredData[j].Contains(tree))
                        isMatch = false;
                }
            }
            return isMatch;
        }

        void AddItem(Filter filter, TreeInfo item)
        {
            if (!FilteredData[(int)filter].Contains(item))
            {
                FilteredData[(int)filter].Add(item);
                if (filter != Filter.Count) hasMatch[(int)filter] = true;
            }
        }

        private void ClearFilteredData()
        {
            for (int i = 0; i < FilteredData.Length; i++)
            {
                FilteredData[i].Clear();
            }
            Data.Clear();
            _hasMatch = null;
        }

        public void FilterTreeListExclusive(string filterText)
        {
            if (ForestBrush.Instance.Trees == null) return;
            string[] filters = filterText?.Trim()?.ToLower().Split(' ');
            List<TreeInfo> treeList = ForestBrush.Instance.Trees.Values.ToList();
            ClearFilteredData();
            if (filters != null && filters.Length > 0 && !string.IsNullOrEmpty(filters[0]))
            {
                var brushTrees = ForestBrush.Instance.BrushTool.TreeInfos;
                var treeAuthors = ForestBrush.Instance.TreeAuthors;
                var treeMeshData = ForestBrush.Instance.TreesMeshData;

                for (int i = 0; i < filters.Length; i++)
                {
                    string filter = filters[i];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        int textureSize = 0;
                        int trisCount = 0;
                        bool isInBrushFilter = filter.Contains("+");
                        bool isNotInBrushFilter = filter.Contains("-");
                        bool isTextureSizeFilter = filter.Length > 2 && filter.Substring(filter.Length - 2).ToLower() == "px";
                        bool isTrisCountFilter = filter.Length > 4 && filter.Substring(filter.Length - 4).ToLower() == "tris";

                        if (isTextureSizeFilter && int.TryParse(filter.Substring(0, filter.Length - 2), out int size))
                            if (size > textureSize) textureSize = size;
                        if (isTrisCountFilter && int.TryParse(filter.Substring(0, filter.Length - 4), out int count))
                            if (count > trisCount) trisCount = count;

                        for (int j = 0; j < treeList.Count; j++)
                        {
                            TreeInfo item = treeList[j];
                            if (item == null) continue;
                            string itemTitle = item.GetUncheckedLocalizedTitle().ToLower();
                            bool itemHasData = treeMeshData.TryGetValue(item.name, out TreeMeshData itemData);
                            bool itemHasAuthor = treeAuthors.TryGetValue(item.name.Split('.')[0], out string itemAuthor);

                            bool isTextureMatch = itemHasData && isTextureSizeFilter && textureSize > 0 && (itemData.textureSize.x <= textureSize && itemData.textureSize.y <= textureSize);
                            bool isTrisMatch = isTrisCountFilter && itemHasData && itemData.triangles > 0 && (itemData.triangles <= trisCount);
                            bool isNotInBrushMatch = isNotInBrushFilter && !brushTrees.Contains(item);
                            bool isInBrushMatch = isInBrushFilter && brushTrees.Contains(item);
                            bool isAuthorMatch = itemHasAuthor && itemAuthor.ToLower().Contains(filter);
                            bool isStringMatch = itemTitle.Contains(filter);

                            if (isTextureMatch)
                            {
                                AddItem(Filter.Texture, item);
                            }
                            if (isTrisMatch)
                            {
                                AddItem(Filter.Tris, item);
                            }
                            if (isNotInBrushMatch)
                            {
                                AddItem(Filter.NotInBrush, item);
                            }
                            if (isInBrushMatch)
                            {
                                AddItem(Filter.InBrush, item);
                            }
                            if (isAuthorMatch)
                            {
                                AddItem(Filter.Author, item);
                            }
                            if (isStringMatch)
                            {
                                AddItem(Filter.String, item);
                            }
                            if (isTextureMatch || isTrisMatch || isNotInBrushMatch || isInBrushMatch || isAuthorMatch || isStringMatch)
                            {
                                AddItem(Filter.Count, item);
                            }
                        }
                    }
                }
                foreach (var tree in FilteredData[(int)Filter.Count])
                {
                    for (int i = 0; i < (int)Filter.Count; i++)
                    {
                        if (IsMatch((Filter)i, tree) && !Data.Contains(tree))
                            Data.Add(tree);
                    }
                }
                treeList = Data;
            }
            treeList.Sort((t1, t2) => t1.CompareTo(t2, UserMod.Settings.Sorting, UserMod.Settings.SortingOrder));
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer = treeList.ToArray();
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_size = treeList.Count;
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.DisplayAt(0f);
        }


        public void FilterTreeListInclusive(string filterText)
        {
            if (ForestBrush.Instance.Trees == null) return;
            string[] filters = filterText?.Trim()?.ToLower().Split(' ');
            List<TreeInfo> treeList = ForestBrush.Instance.Trees.Values.ToList();
            if (filters != null && filters.Length > 0 && !string.IsNullOrEmpty(filters[0]))
            {
                var brushTrees = ForestBrush.Instance.BrushTool.TreeInfos;
                var treeAuthors = ForestBrush.Instance.TreeAuthors;
                var treeMeshData = ForestBrush.Instance.TreesMeshData;
                List<TreeInfo> newData = new List<TreeInfo>();
                bool showBrushTree = false;
                bool showNotBrushTree = false;
                bool textureSizeFilter = false;
                bool trisCountFilter = false;
                int textureSize = 0;
                int trisCount = 0;

                for (int i = 0; i < filters.Length; i++)
                {
                    string filter = filters[i];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        showBrushTree = filter.Contains("+");
                        showNotBrushTree = filter.Contains("-");
                        textureSizeFilter = filter.Length > 2 && filter.Substring(filter.Length - 2).ToLower() == "px";
                        trisCountFilter = filter.Length > 4 && filter.Substring(filter.Length - 4).ToLower() == "tris";
                        if (textureSizeFilter && int.TryParse(filter.Substring(0, filter.Length - 2), out int size))
                            if (size > textureSize) textureSize = size;
                        if (trisCountFilter && int.TryParse(filter.Substring(0, filter.Length - 4), out int count))
                            if (count > trisCount) trisCount = count;
                    }
                }
                for (int i = 0; i < filters.Length; i++)
                {
                    string filter = filters[i];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        for (int j = 0; j < treeList.Count; j++)
                        {
                            TreeInfo item = treeList[j];
                            if (item == null) continue;
                            string itemTitle = item.GetUncheckedLocalizedTitle().ToLower();
                            bool itemHasData = treeMeshData.TryGetValue(item.name, out TreeMeshData itemData);
                            bool itemHasAuthor = treeAuthors.TryGetValue(item.name.Split('.')[0], out string itemAuthor);
                            if ((textureSizeFilter && itemHasData && textureSize > 0 && (itemData.textureSize.x <= textureSize && itemData.textureSize.y <= textureSize))
                            || (trisCountFilter && itemHasData && itemData.triangles > 0 && (itemData.triangles <= trisCount))
                            || (showNotBrushTree && !brushTrees.Contains(item))
                            || (showBrushTree && brushTrees.Contains(item))
                            || (itemHasAuthor && itemAuthor.ToLower().Contains(filter))
                            || (itemTitle.Contains(filter)))
                            {
                                if (!newData.Contains(item)) newData.Add(item);
                            }
                        }
                    }
                }
                treeList = newData;
            }
            treeList.Sort((t1, t2) => t1.CompareTo(t2, UserMod.Settings.Sorting, UserMod.Settings.SortingOrder));
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer = treeList.ToArray();
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_size = treeList.Count;
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.DisplayAt(0f);
        }

        public void FilterTreeListBasic(string filterText)
        {
            if (ForestBrush.Instance.Trees == null) return;
            string[] filters = filterText?.Trim()?.ToLower().Split(' ');
            List<TreeInfo> treeList = ForestBrush.Instance.Trees.Values.ToList();
            if (filters != null && filters.Length > 0 && !string.IsNullOrEmpty(filters[0]))
            {
                var newData = new List<TreeInfo>();
                for (int i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];
                    if (!string.IsNullOrEmpty(filter))
                    {
                        for (int j = 0; j < treeList.Count; j++)
                        {
                            var item = treeList[j];
                            if (item == null) continue;
                            if (item.GetUncheckedLocalizedTitle().ToLower().Contains(filter))
                            {
                                if (!newData.Contains(item)) newData.Add(item);
                            }
                        }
                    }
                }
                treeList = newData;
            }
            treeList.Sort((t1, t2) => t1.CompareTo(t2, UserMod.Settings.Sorting, UserMod.Settings.SortingOrder));
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer = treeList.ToArray();
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_size = treeList.Count;
            ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.DisplayAt(0f);
        }
    }
}