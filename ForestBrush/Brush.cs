using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    [Serializable]
    public class Brush
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

        public static Brush Default()
        {
            return new Brush()
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
            public string BitmapID { get; set; }

            public static BrushOptions Default()
            {
                return new BrushOptions()
                {
                    Size = 150f,
                    Strength = 0.2f,
                    Density = 8f,
                    AutoDensity = true,
                    BitmapID = string.Empty
                };
            }
        }
    }
}