using ColossalFramework.UI;
using ForestBrush.GUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush
{
    public class ForestBrushTool : MonoBehaviour
    {
        public ForestBrush Brush => ForestBrushMod.instance.Settings.SelectedBrush;

        private List<TreeInfo> TreeInfos { get; set; } = new List<TreeInfo>();

        private TreeInfo Container { get; set; } = ForestBrushMod.instance.Container;

        public Dictionary<string, ForestBrush> Brushes => ForestBrushMod.instance.Settings.Brushes;
               
        void Awake()
        {
            UpdateTool(ForestBrushMod.instance.Settings.SelectedBrush.Name);
        }

        public void UpdateTool(string brushName)
        {
            ForestBrushMod.instance.Settings.SelectBrush(brushName);

            TreeInfos = new List<TreeInfo>();

            foreach (var tree in Brush.Trees)
            {
                var treeInfo = ForestBrushMod.instance.Trees[tree.Name];
                if (!treeInfo) continue;
                TreeInfos.Add(treeInfo);
            }

            ForestBrushMod.instance.ForestBrushPanel.LoadBrush(Brush);

            Container = CreateBrushPrefab();

            ForestBrushMod.instance.SaveSettings();
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
            var infoBuffer = ForestBrushMod.instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer;
            var itemBuffer = ForestBrushMod.instance.ForestBrushPanel.BrushEditSection.TreesList.rows.m_buffer;
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
            var infoBuffer = ForestBrushMod.instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer.Cast<TreeInfo>().ToList();
            TreeInfos = ForestBrushMod.instance.Trees.Values.Where(treeInfo => infoBuffer.Contains(treeInfo)).ToList();
            Brush.ReplaceAll(TreeInfos);
            var itemBuffer = ForestBrushMod.instance.ForestBrushPanel.BrushEditSection.TreesList.rows.m_buffer;
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(true);
            }
        }

        public void New(string brushName)
        {
            if (!Brushes.TryGetValue(brushName, out ForestBrush brush))
            {
                brush = ForestBrush.Default();

                brush.ReplaceAll(TreeInfos);

                Brushes.Add(brushName, brush);

                ForestBrushMod.instance.Settings.SelectBrush(brushName);

                ForestBrushMod.instance.ForestBrushPanel.BrushSelectSection.UpdateDropDown();

                ForestBrushMod.instance.SaveSettings();
            }                
            else UIView.PushModal(NewBrushModal.Instance);
        }

        internal void DeleteCurrent()
        {
            Brushes.Remove(Brush.Name);
            ForestBrushMod.instance.Settings.SelectNextBestBrush();
            ForestBrushMod.instance.ForestBrushPanel.BrushSelectSection.UpdateDropDown();
            string nextBrush = ForestBrushMod.instance.ForestBrushPanel.BrushSelectSection.SelectBrushDropDown.items.Length <= 0 ? Constants.NewBrushName :
                ForestBrushMod.instance.ForestBrushPanel.BrushSelectSection.SelectBrushDropDown.selectedValue;
            UpdateTool(nextBrush);
            ForestBrushMod.instance.SaveSettings();
        }

        public TreeInfo CreateBrushPrefab()
        {   
            var variations = new TreeInfo.Variation[TreeInfos.Count];
            if (TreeInfos.Count == 0)
            {
                Container.m_variations = variations;
                return Container;
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
            Container.m_variations = variations;
            return Container;
        }

        internal void UpdateTreeList(TreeInfo treeInfo, bool value, bool updateAll)
        {
            if (value) Add(treeInfo);
            else Remove(treeInfo);
            if(updateAll)
            {
                if (value) AddAll();
                else RemoveAll();
            }
            Container = CreateBrushPrefab();
        }
    }
}