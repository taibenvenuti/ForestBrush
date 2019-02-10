using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        }

        public void Remove(TreeInfo treeInfo)
        {
            var name = treeInfo.name;
            Tree tree = Trees.Find(t => t.Name == name);
            Trees.Remove(tree);
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
        }

        public static ForestBrush Default()
        {
            return new ForestBrush()
            {
                Name = Constants.NewBrushName,
                Trees = new List<Tree>(),
                Options = BrushOptions.Default()
            };
        }

        [Serializable]
        public class BrushOptions
        {
            public float Size { get; set; }
            public float Strength { get; set; }
            public float Density { get; set; }
            public bool AutoDensity { get; set; }
            public bool IsSquare { get; set; }
            public OverlayColor OverlayColor { get; set; }

            public static BrushOptions Default()
            {
                return new BrushOptions()
                {
                    Size = 100f,
                    Strength = 0.1f,
                    Density = 1f,
                    AutoDensity = true,
                    IsSquare = false,
                    OverlayColor = new Color32(25, 125, 155, 255)
                };
            }
        }
    }
}