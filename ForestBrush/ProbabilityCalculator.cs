using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestBrush
{
    public class ProbabilityCalculator
    {
        /// <summary>
        /// Calculates the probabilities for all trees based on weight.
        /// </summary>
        /// <param name="trees">Trees to calculate the probability for.</param>
        /// <returns>A list of the trees with probabilities.</returns>
        public List<TreeProbability> Calculate(List<Tree> trees)
        {
            if (trees.Count == 0)
            {
                return new List<TreeProbability>();
            }

            if (trees.Count > 100)
            {
                throw new Exception("Tree count must be lower or equal to 100.");
            }

            var weightSum = 0f;
            for (int i = 0; i < trees.Count; i++)
            {
                weightSum += trees[i].Probability;
            }

            if (weightSum <= 0f)
            {
                throw new Exception("Sum of all weights must be larger than 0.");
            }

            var result = new List<TreeProbability>(trees.Count);
            for (var i = 0; i < trees.Count; i++)
            {
                var tree = trees[i];
                var probability = tree.Probability / weightSum;
                var floorProbability = Convert.ToInt32(Math.Floor(probability * 100));
                if (floorProbability < 1)
                {
                    floorProbability = 1;
                }

                result.Add(new TreeProbability(tree.Name, probability, floorProbability));
            }

            var floorProbabilitySum = 0;
            for (int i = 0; i < trees.Count; i++)
            {
                floorProbabilitySum += result[i].FloorProbability;
            }

            while (floorProbabilitySum != 100)
            {
                int correction = (floorProbabilitySum < 100) ? 1 : -1;

                // find the tree which deserves the increase or decrease in probability
                int biggestErrorIndex = -1;
                float biggestError = float.MinValue;
                for (var i = 0; i < trees.Count; i++)
                {
                    var tree = result[i];

                    // do not go below 1
                    if (correction == -1 && tree.FloorProbability == 1)
                    {
                        continue;
                    }

                    var error = (tree.Probability - tree.FloorProbability / 100f) * correction;
                    if (error > biggestError)
                    {
                        biggestErrorIndex = i;
                        biggestError = error;
                    }
                }

                if (biggestErrorIndex == -1)
                {
                    throw new Exception("Biggest Error Index was -1");
                }

                var currentItem = result[biggestErrorIndex];

                result[biggestErrorIndex] = new TreeProbability(currentItem.Name, currentItem.Probability, currentItem.FloorProbability + correction);
                floorProbabilitySum += correction;
            }

            return result;
        }
    }
}
