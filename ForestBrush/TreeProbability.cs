namespace ForestBrush
{
    public struct TreeProbability
    {
        public TreeProbability(string name, float probability, int floorProbability)
        {
            Name = name;
            Probability = probability;
            FloorProbability = floorProbability;
        }

        public string Name { get; }
        public float Probability { get; }
        public int FloorProbability { get; }
    }
}
    