using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    [Serializable]
    public class ForestBrush
    {
        public string Name { get; set; }

        public List<Tree> Trees { get; set; }

        public BrushOptions Options { get; set; }

        public void Add(TreeInfo treeInfo)
        {
            string name = treeInfo.name;
            if (!Trees.Any(t => t.Name == name))
                Trees.Add(new Tree(treeInfo));
            Options.Capture();
        }

        public void Remove(TreeInfo treeInfo)
        {
            var name = treeInfo.name;
            Tree tree = Trees.Find(t => t.Name == name);
            Trees.Remove(tree);
            Options.Capture();
        }

        /// <summary>
        /// Replaces all current trees in this brush with the ones provided.
        /// Also updates the brush options to currently selected options.
        /// </summary>
        /// <param name="newTrees">A list of TreeInfo trees to replace the existing ones with.</param>
        public void ReplaceAll(List<TreeInfo> newTrees)
        {
            Trees = new List<Tree>();
            foreach (var treeInfo in newTrees)
            {
                Tree tree = new Tree(treeInfo);
                Trees.Add(tree);
            }
            Options.Capture();
        }

        [Serializable]
        public struct BrushOptions
        {
            public float Size;
            public float Strength;
            public float Density;
            public bool AutoDensity;
            public bool IsSquare;
            public OverlayColor OverlayColor;

            public void Capture()
            {
                Size = ForestBrushMod.instance.Settings.BrushSize;
                Strength = ForestBrushMod.instance.Settings.BrushStrength;
                Density = ForestBrushMod.instance.Settings.BrushDensity;
                AutoDensity = ForestBrushMod.instance.Settings.AutoDensity;
                IsSquare = ForestBrushMod.instance.Settings.SquareBrush;
                OverlayColor = ForestBrushMod.instance.BrushSettings.OverlayColor;
            }
        }
    }
}