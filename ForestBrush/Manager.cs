using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ForestBrush.GUI;
using ICities;

namespace ForestBrush
{           
    public class ForestBrushes : Singleton<ForestBrushes>
    {
        internal AppMode AppMode { get; set; }

        UIComponent treePanel;

        UIComponent findItPanel;

        internal ForestBrushPanel BrushPanel;

        TreeBrushTool brushTool;

        internal TreeInfo Container;

        public Dictionary<string, TreeInfo> Trees { get; set; } = new Dictionary<string, TreeInfo>();

        public Dictionary<string, List<string>> Brushes { get; set; } = new Dictionary<string, List<string>>();

        public TreeBrushTool BrushTool { get => brushTool; set => brushTool = value; }

        internal void Initialize()
        {
            ForestBrushPerks.Initialize();
            Trees = LoadAllTrees();
            Container = PrefabCollection<TreeInfo>.FindLoaded("ForestBrushContainer.Forest Brush_Data");
            if (Container == null) return;
            treePanel = AppMode == AppMode.Game ? UIView.Find<UIComponent>("LandscapingTreesPanel") : UIView.Find<UIComponent>("ForestDefaultPanel");
            findItPanel = UIView.Find<UIComponent>("FindItDefaultPanel");
            treePanel.eventVisibilityChanged += (c, e) =>
            {
                if (!e && BrushPanel) DestroyImmediate(BrushPanel.gameObject);
                BrushPanel = null;
            };
            if(findItPanel != null)
            {
                findItPanel.eventVisibilityChanged += (c, e) =>
                {
                    if (!e && BrushPanel) DestroyImmediate(BrushPanel.gameObject);
                    BrushPanel = null;
                };
            }
            Brushes.TryGetValue(UserMod.Settings.SelectedBrush, out List<string> brush);
            brushTool = new TreeBrushTool(UserMod.Settings.SelectedBrush, UserMod.Settings.SelectedBrush == Constants.VanillaPack ? null : brush);               
        }

        private Dictionary<string, TreeInfo> LoadAllTrees()
        {
            var trees = new Dictionary<string, TreeInfo>();
            var treeCount = PrefabCollection<TreeInfo>.LoadedCount();
            for (uint i = 0; i < treeCount; i++)
            {
                var tree = PrefabCollection<TreeInfo>.GetLoaded(i);
                if (tree == null || tree == Container || (!UserMod.IsCherryUnlocked() && tree.name == "CherryTree01")) continue;
                trees.Add(tree.name, tree);
            }
            return trees;
        }

        void Update()
        {
            if (BrushPanel != null || brushTool?.Container == null || !(ToolsModifierControl.toolController?.CurrentTool is TreeTool) || ((TreeTool)ToolsModifierControl.toolController?.CurrentTool)?.m_prefab != brushTool?.Container) return;
            
            BrushPanel = UIView.GetAView()?.AddUIComponent(typeof(ForestBrushPanel)) as ForestBrushPanel;
        }


        internal void UpdateBrushItems(TreeInfo treeInfo, bool value, bool updateAll)
        {
            brushTool.Update(treeInfo, value, updateAll);
        }
    }
}
