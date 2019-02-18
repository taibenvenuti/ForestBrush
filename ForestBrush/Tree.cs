namespace ForestBrush
{
    public class Tree
    {
        public string Name;
        public float Probability;

        public Tree()
        {

        }

        public Tree(TreeInfo tree)
        {
            Name = tree.name;
            Probability = 100f;
        }
    }
}
