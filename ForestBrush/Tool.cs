using ForestBrush.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    public class ForestBrushTool
    {
        public ForestBrush Brush { get; set; }

        private List<TreeInfo> TreeInfos { get; set; } = new List<TreeInfo>();

        public TreeInfo Container { get; private set; }

        public List<ForestBrush> SavedBrushes => UserMod.BrushSettings.SavedBrushes;

        public ForestBrushTool()
        {
            UpdateTool(CGSSerialized.SelectedBrush);
        }

        public void UpdateTool(string name)
        {
            Container = ForestBrushMod.instance.Container;

            if (!Container) return;

            Brush = SavedBrushes.Find(brush => brush.Name == name);

            if (Brush == null)
            {
                Brush = new ForestBrush();
                CODebug.Log(LogChannel.Modding, "Brush was null, creating new vanilla brush.");
            }

            if (Brush.Name == Constants.VanillaPack)
            {
                TreeInfos = ForestBrushMod.instance.Trees.Values.Where(t => t.m_isCustomContent == false).ToList();                
            }
            else
            {
                TreeInfos = new List<TreeInfo>();
                foreach (var treeName in Brush.Trees)
                {
                    var tree = ForestBrushMod.instance.Trees[treeName];
                    if (!tree) continue;
                    TreeInfos.Add(tree);
                }
            }

            Container = SetBrushActive(Container);
        }

        private void Add(TreeInfo tree)
        {
            if (TreeInfos.Contains(tree)) return;
            TreeInfos.Add(tree);
            Brush.Add(tree);
        }

        private void Remove(TreeInfo tree)
        {
            if (!TreeInfos.Contains(tree)) return;
            TreeInfos.Remove(tree);
            Brush.Remove(tree);
        }

        public void RemoveAll()
        {
            var infoBuffer = ForestBrushMod.instance.ForestBrushPanel.TreesList.rowsData.m_buffer;
            var itemBuffer = ForestBrushMod.instance.ForestBrushPanel.TreesList.rows.m_buffer;
            foreach (TreeInfo tree in infoBuffer)
            {
                Remove(tree);
            }
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(false);
            }
        }

        private void AddAll()
        {
            var infoBuffer = ForestBrushMod.instance.ForestBrushPanel.TreesList.rowsData.m_buffer.Cast<TreeInfo>().ToList();
            TreeInfos = ForestBrushMod.instance.Trees.Values.Where(treeInfo => infoBuffer.Contains(treeInfo)).ToList();
            var treeNames = TreeInfos.Select(p => p.name).ToList();
            Brush.Update(treeNames);
            var itemBuffer = ForestBrushMod.instance.ForestBrushPanel.TreesList.rows.m_buffer;
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(true);
            }
        }

        internal void Save()
        {
            if (Brush.Name == Constants.VanillaPack)
            {
                throw new NotImplementedException();
            }
            else UserMod.BrushSettings.Save();
        }

        public void New(string brushName)
        {
            List<string> newTreeNames = TreeInfos.Select(p => p.name).ToList();
            bool brushExists = SavedBrushes.Find(b => b.Name == brushName) != null; 
            if (!brushExists)
            {
                SavedBrushes.Add(new ForestBrush(brushName, newTreeNames));
                CGSSerialized.SelectedBrush.value = brushName;
                ForestBrushMod.instance.ForestBrushPanel.UpdateDropDown();
                UserMod.BrushSettings.Save();
            }                
            else ForestBrushMod.instance.ForestBrushPanel.OnSaveCurrentClickedEventHandler(true);
        }

        internal void DeleteCurrent()
        {
            SavedBrushes.Remove(Brush);
            CGSSerialized.SelectedBrush.value = Constants.VanillaPack;
            UserMod.BrushSettings.Save();
            ForestBrushMod.instance.ForestBrushPanel.UpdateDropDown();
        }

        public TreeInfo SetBrushActive(TreeInfo info)
        {   
            var variations = new TreeInfo.Variation[TreeInfos.Count];
            if (TreeInfos.Count == 0)
            {
                info.m_variations = variations;
                return info;
            }
            for (int i = 0; i < TreeInfos.Count; i++)
            {
                var variation = new TreeInfo.Variation();
                variation.m_tree = variation.m_finalTree = TreeInfos[i];
                variation.m_probability = 100 / TreeInfos.Count;
                variations[i] = variation;
            }
            int index = 0;
            int remainder = 100 % TreeInfos.Count;
            while(remainder > 0)
            {
                variations[index].m_probability++;
                index++;
                remainder--;
                if(remainder > 0 && index == TreeInfos.Count)
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
            Container = SetBrushActive(Container);
        }
    }
}