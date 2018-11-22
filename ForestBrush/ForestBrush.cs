using System;
using System.Collections.Generic;

namespace ForestBrush
{
    [Serializable]
    public class ForestBrush
    {
        public string Name { get; private set; }

        public List<string> Trees { get; private set; }

        public ForestBrush()
        {
            Name = Constants.VanillaPack;
            Trees = new List<string>();
        }

        public ForestBrush(string name)
        {
            Name = name;
            Trees = new List<string>();
        }

        public ForestBrush(string name, List<string> trees)
        {
            Name = name;
            Trees = trees;
        }

        public void Add(TreeInfo tree)
        {
            var name = tree.GetUncheckedLocalizedTitle();
            if (!Trees.Contains(name))
                Trees.Add(name);
        }

        public void Remove(TreeInfo tree)
        {
            var name = tree.GetUncheckedLocalizedTitle();
            Trees.Remove(name);
        }

        public void Update(List<string> newTrees)
        {
            Trees = newTrees ?? Trees;
        }
    }
}
