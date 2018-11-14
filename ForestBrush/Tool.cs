using ForestBrush.GUI;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    public class TreeBrushTool
    {
        string brushName;

        List<TreeInfo> treeInfos;

        TreeInfo container;

        public TreeInfo Container => container;

        public TreeBrushTool()
        {
            treeInfos = new List<TreeInfo>();
        }

        public TreeBrushTool(string name, List<string> treeNames = null)
        {
            container = ForestBrushes.instance.Container;

            brushName = name;

            if (!container) return;

            if(treeNames == null)
                treeInfos = ForestBrushes.instance.Trees.Values.Where(t => t.m_isCustomContent == false).ToList();
            else
            {
                treeInfos = new List<TreeInfo>();
                foreach (var treeName in treeNames)
                {
                    var tree = ForestBrushes.instance.Trees[treeName];
                    if (!tree) continue;
                    treeInfos.Add(tree);
                }
            }               

            container = CreateBrush(container);
        }

        private void Add(TreeInfo tree)
        {
            if (treeInfos.Contains(tree)) return;
            treeInfos.Add(tree);
        }

        private void Remove(TreeInfo tree)
        {
            if (!treeInfos.Contains(tree)) return;
            treeInfos.Remove(tree);
        }

        public void RemoveAll()
        {
            var infoBuffer = ForestBrushes.instance.BrushPanel.TreesList.rowsData.m_buffer;
            var itemBuffer = ForestBrushes.instance.BrushPanel.TreesList.rows.m_buffer;
            foreach (TreeInfo tree in infoBuffer)
            {
                if (treeInfos.Contains(tree))
                    treeInfos.Remove(tree);
            }
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(false);
            }
        }

        private void AddAll()
        {
            var infoBuffer = ForestBrushes.instance.BrushPanel.TreesList.rowsData.m_buffer.Cast<TreeInfo>().ToList();
            treeInfos = ForestBrushes.instance.Trees.Values.Where(treeInfo => infoBuffer.Contains(treeInfo)).ToList();
            var itemBuffer = ForestBrushes.instance.BrushPanel.TreesList.rows.m_buffer;
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(true);
            }
        }

        internal void Save()
        {            
            var newTreeNames = treeInfos.Select(p => p.name).ToList();
            if (!ForestBrushes.instance.Brushes.TryGetValue(brushName, out List<string> treeNames))
                ForestBrushes.instance.Brushes.Add(brushName, newTreeNames);
            else ForestBrushes.instance.Brushes[brushName] = newTreeNames;

            UserMod.Settings.Save();
        }

        public void New(string brushName)
        {
            var newTreeNames = treeInfos.Select(p => p.name).ToList();
            if (!ForestBrushes.instance.Brushes.TryGetValue(brushName, out List<string> treeNames))
            {
                ForestBrushes.instance.Brushes.Add(brushName, newTreeNames);
                UserMod.Settings.Save();
            }                
            else ForestBrushes.instance.BrushPanel.OnSaveCurrentClickedEventHandler(true);
            UserMod.Settings.SelectedBrush = brushName;
            ForestBrushes.instance.BrushPanel.UpdateDropDown();
        }

        internal void Delete()
        {            
            if (ForestBrushes.instance.Brushes.TryGetValue(brushName, out List<string> treeNames))
                ForestBrushes.instance.Brushes.Remove(brushName);
            UserMod.Settings.Save();
            ForestBrushes.instance.BrushPanel.UpdateDropDown();
        }

        public TreeInfo CreateBrush(TreeInfo info)
        {   
            var variations = new TreeInfo.Variation[treeInfos.Count];
            if (treeInfos.Count == 0)
            {
                info.m_variations = variations;
                return info;
            }
            for (int i = 0; i < treeInfos.Count; i++)
            {
                var variation = new TreeInfo.Variation();
                variation.m_tree = variation.m_finalTree = treeInfos[i];
                variation.m_probability = 100 / treeInfos.Count;
                variations[i] = variation;
            }
            int index = 0;
            int remainder = 100 % treeInfos.Count;
            while(remainder > 0)
            {
                variations[index].m_probability++;
                index++;
                remainder--;
                if(remainder > 0 && index == treeInfos.Count)
                {
                    index = 0;
                }
            }
            info.m_variations = variations;
            return info;
        }

        internal void Update(TreeInfo treeInfo, bool value, bool updateAll)
        {
            if (value) Add(treeInfo);
            else Remove(treeInfo);
            if(updateAll)
            {
                if (value) AddAll();
                else RemoveAll();
            }
            container = CreateBrush(container);
        }
    }
}