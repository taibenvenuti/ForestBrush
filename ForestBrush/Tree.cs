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
            Probability = GetProbability(tree);
        }

        private float GetProbability(TreeInfo treeInfo)
        {
            float probability = 10f;
            string savedProbabilityData = treeInfo.m_mesh.name;
            if (savedProbabilityData.Contains("Probability:"))
            {
                savedProbabilityData = savedProbabilityData.Replace("Probability:", "");
                float.TryParse(savedProbabilityData, out probability);
            }
            return probability;
        }
    }
}
