using ForestBrush.GUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForestBrush
{
    public class ForestBrushTool : MonoBehaviour
    {
        public Brush Brush => UserMod.Settings.SelectedBrush;

        private List<TreeInfo> TreeInfos { get; set; } = new List<TreeInfo>();

        private TreeInfo Container { get; set; } = ForestBrush.Instance.Container;

        public List<Brush> Brushes => UserMod.Settings.Brushes;

        void Awake()
        {
            UpdateTool(UserMod.Settings.SelectedBrush.Name);
        }

        public void UpdateTool(string brushName)
        {
            UserMod.Settings.SelectBrush(brushName);

            TreeInfos = new List<TreeInfo>();

            foreach (var tree in Brush.Trees)
            {
                if (!ForestBrush.Instance.Trees.TryGetValue(tree.Name, out TreeInfo treeInfo)) continue;
                if (treeInfo == null) continue;
                TreeInfos.Add(treeInfo);
            }

            Container = CreateBrushPrefab();

            ForestBrush.Instance.ForestBrushPanel.LoadBrush(Brush);

            UserMod.SaveSettings();
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
            var infoBuffer = ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer;
            var itemBuffer = ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rows.m_buffer;
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
            var infoBuffer = ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rowsData.m_buffer.Cast<TreeInfo>().ToList();
            TreeInfos = ForestBrush.Instance.Trees.Values.Where(treeInfo => infoBuffer.Contains(treeInfo)).ToList();
            Brush.ReplaceAll(TreeInfos);
            var itemBuffer = ForestBrush.Instance.ForestBrushPanel.BrushEditSection.TreesList.rows.m_buffer;
            foreach (TreeItem item in itemBuffer)
            {
                item?.ToggleCheckbox(true);
            }
        }

        public void New(string brushName)
        {
            if (Brushes.Find(b => b.Name == brushName) == null)
            {
                Brush brush = Brush.Default();

                brush.Name = brushName;

                brush.ReplaceAll(TreeInfos);

                Brushes.Add(brush);

                UpdateTool(brushName);
            }
            else Debug.LogError("Error creatin new brush. Brush already exists. This shouldn't happen, please contact the mod author.");
        }

        internal void DeleteCurrent()
        {
            Brushes.Remove(Brush);
            UserMod.Settings.SelectNextBestBrush();
            ForestBrush.Instance.ForestBrushPanel.BrushSelectSection.UpdateDropDown();
            string nextBrush = ForestBrush.Instance.ForestBrushPanel.BrushSelectSection.SelectBrushDropDown.items.Length <= 0 ? Constants.NewBrushName :
                ForestBrush.Instance.ForestBrushPanel.BrushSelectSection.SelectBrushDropDown.selectedValue;
            UpdateTool(nextBrush);
            UserMod.SaveSettings();
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
            UserMod.SaveSettings();
        }
    }
}