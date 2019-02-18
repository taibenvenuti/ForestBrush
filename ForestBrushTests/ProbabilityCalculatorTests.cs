using FluentAssertions;
using ForestBrush;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ForestBrushTests
{
    public class ProbabilityCalculatorTests
    {
        [Fact]
        public void Calculate_NoTree_SouldReturnEmptyList()
        {
            var trees = new List<Tree>();
            var expected = new List<TreeProbability>();
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_OneTree_SouldBe100Percent()
        {
            var trees = new List<Tree>()
            {
                new Tree() { Name = "tree1", Probability = 50 },
            };
            var expected = new List<TreeProbability>()
            {
                new TreeProbability("tree1", 1f, 100),
            };
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_MultipleTrees_SameProbability_ShouldBeDistributedEvenly()
        {
            var trees = new List<Tree>()
            {
                new Tree() { Name = "tree1", Probability = 30 },
                new Tree() { Name = "tree2", Probability = 30 },
                new Tree() { Name = "tree3", Probability = 30 },
                new Tree() { Name = "tree4", Probability = 30 },
                new Tree() { Name = "tree5", Probability = 30 },
            };
            var expected = new List<TreeProbability>()
            {
                new TreeProbability("tree1", 0.2f, 20),
                new TreeProbability("tree2", 0.2f, 20),
                new TreeProbability("tree3", 0.2f, 20),
                new TreeProbability("tree4", 0.2f, 20),
                new TreeProbability("tree5", 0.2f, 20),
            };
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_MultipleTrees_DiffProbabilites_ShouldBeDistributedInCorrectRatio_1()
        {
            var trees = new List<Tree>()
            {
                new Tree() { Name = "tree1", Probability = 50 },
                new Tree() { Name = "tree2", Probability = 50 },
                new Tree() { Name = "tree3", Probability = 100 },
            };
            var expected = new List<TreeProbability>()
            {
                new TreeProbability("tree1", 0.25f, 25),
                new TreeProbability("tree2", 0.25f, 25),
                new TreeProbability("tree3", 0.50f, 50),
            };
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_MultipleTrees_DiffProbabilites_ShouldBeDistributedInCorrectRatio_2()
        {
            var trees = new List<Tree>()
            {
                new Tree() { Name = "tree1", Probability = 100 },
                new Tree() { Name = "tree2", Probability = 10 },
            };
            var sum = trees.Sum(x => x.Probability);
            var expected = new List<TreeProbability>()
            {
                new TreeProbability("tree1", 100 / sum, 91),
                new TreeProbability("tree2", 10 / sum, 9),
            };
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_100Trees_DiffProbabilites_ShouldBeSameRatioToAllowMaxNumberOfTrees()
        {
            var trees = new List<Tree>()
            {
                new Tree() { Name = "tree1", Probability = 1 },
                new Tree() { Name = "tree2", Probability = 1 },
                new Tree() { Name = "tree3", Probability = 1 },
                new Tree() { Name = "tree4", Probability = 1 },
                new Tree() { Name = "tree5", Probability = 1 },
                new Tree() { Name = "tree6", Probability = 1 },
                new Tree() { Name = "tree7", Probability = 1 },
                new Tree() { Name = "tree8", Probability = 1 },
                new Tree() { Name = "tree9", Probability = 1 },
                new Tree() { Name = "tree10", Probability = 1 },
                new Tree() { Name = "tree11", Probability = 1 },
                new Tree() { Name = "tree12", Probability = 1 },
                new Tree() { Name = "tree13", Probability = 1 },
                new Tree() { Name = "tree14", Probability = 1 },
                new Tree() { Name = "tree15", Probability = 1 },
                new Tree() { Name = "tree16", Probability = 1 },
                new Tree() { Name = "tree17", Probability = 1 },
                new Tree() { Name = "tree18", Probability = 1 },
                new Tree() { Name = "tree19", Probability = 1 },
                new Tree() { Name = "tree20", Probability = 1 },
                new Tree() { Name = "tree21", Probability = 1 },
                new Tree() { Name = "tree22", Probability = 1 },
                new Tree() { Name = "tree23", Probability = 1 },
                new Tree() { Name = "tree24", Probability = 1 },
                new Tree() { Name = "tree25", Probability = 1 },
                new Tree() { Name = "tree26", Probability = 1 },
                new Tree() { Name = "tree27", Probability = 1 },
                new Tree() { Name = "tree28", Probability = 1 },
                new Tree() { Name = "tree29", Probability = 1 },
                new Tree() { Name = "tree30", Probability = 1 },
                new Tree() { Name = "tree31", Probability = 1 },
                new Tree() { Name = "tree32", Probability = 1 },
                new Tree() { Name = "tree33", Probability = 1 },
                new Tree() { Name = "tree34", Probability = 1 },
                new Tree() { Name = "tree35", Probability = 1 },
                new Tree() { Name = "tree36", Probability = 1 },
                new Tree() { Name = "tree37", Probability = 1 },
                new Tree() { Name = "tree38", Probability = 1 },
                new Tree() { Name = "tree39", Probability = 1 },
                new Tree() { Name = "tree40", Probability = 1 },
                new Tree() { Name = "tree41", Probability = 1 },
                new Tree() { Name = "tree42", Probability = 1 },
                new Tree() { Name = "tree43", Probability = 1 },
                new Tree() { Name = "tree44", Probability = 1 },
                new Tree() { Name = "tree45", Probability = 1 },
                new Tree() { Name = "tree46", Probability = 1 },
                new Tree() { Name = "tree47", Probability = 1 },
                new Tree() { Name = "tree48", Probability = 1 },
                new Tree() { Name = "tree49", Probability = 1 },
                new Tree() { Name = "tree50", Probability = 1 },
                new Tree() { Name = "tree51", Probability = 1 },
                new Tree() { Name = "tree52", Probability = 1 },
                new Tree() { Name = "tree53", Probability = 1 },
                new Tree() { Name = "tree54", Probability = 1 },
                new Tree() { Name = "tree55", Probability = 1 },
                new Tree() { Name = "tree56", Probability = 1 },
                new Tree() { Name = "tree57", Probability = 1 },
                new Tree() { Name = "tree58", Probability = 1 },
                new Tree() { Name = "tree59", Probability = 1 },
                new Tree() { Name = "tree60", Probability = 1 },
                new Tree() { Name = "tree61", Probability = 1 },
                new Tree() { Name = "tree62", Probability = 1 },
                new Tree() { Name = "tree63", Probability = 1 },
                new Tree() { Name = "tree64", Probability = 1 },
                new Tree() { Name = "tree65", Probability = 1 },
                new Tree() { Name = "tree66", Probability = 1 },
                new Tree() { Name = "tree67", Probability = 1 },
                new Tree() { Name = "tree68", Probability = 1 },
                new Tree() { Name = "tree69", Probability = 1 },
                new Tree() { Name = "tree70", Probability = 1 },
                new Tree() { Name = "tree71", Probability = 1 },
                new Tree() { Name = "tree72", Probability = 1 },
                new Tree() { Name = "tree73", Probability = 1 },
                new Tree() { Name = "tree74", Probability = 1 },
                new Tree() { Name = "tree75", Probability = 1 },
                new Tree() { Name = "tree76", Probability = 1 },
                new Tree() { Name = "tree77", Probability = 1 },
                new Tree() { Name = "tree78", Probability = 1 },
                new Tree() { Name = "tree79", Probability = 1 },
                new Tree() { Name = "tree80", Probability = 1 },
                new Tree() { Name = "tree81", Probability = 1 },
                new Tree() { Name = "tree82", Probability = 1 },
                new Tree() { Name = "tree83", Probability = 1 },
                new Tree() { Name = "tree84", Probability = 1 },
                new Tree() { Name = "tree85", Probability = 1 },
                new Tree() { Name = "tree86", Probability = 1 },
                new Tree() { Name = "tree87", Probability = 1 },
                new Tree() { Name = "tree88", Probability = 1 },
                new Tree() { Name = "tree89", Probability = 1 },
                new Tree() { Name = "tree90", Probability = 1 },
                new Tree() { Name = "tree91", Probability = 1 },
                new Tree() { Name = "tree92", Probability = 1 },
                new Tree() { Name = "tree93", Probability = 1 },
                new Tree() { Name = "tree94", Probability = 50 },
                new Tree() { Name = "tree95", Probability = 50 },
                new Tree() { Name = "tree96", Probability = 100 },
                new Tree() { Name = "tree97", Probability = 100 },
                new Tree() { Name = "tree98", Probability = 100 },
                new Tree() { Name = "tree99", Probability = 100 },
                new Tree() { Name = "tree100", Probability = 100 },
            };
            var sum = trees.Sum(x => x.Probability);
            var expected = new List<TreeProbability>()
            {
                new TreeProbability("tree1", 1 / sum, 1),
                new TreeProbability("tree2", 1 / sum, 1),
                new TreeProbability("tree3", 1 / sum, 1),
                new TreeProbability("tree4", 1 / sum, 1),
                new TreeProbability("tree5", 1 / sum, 1),
                new TreeProbability("tree6", 1 / sum, 1),
                new TreeProbability("tree7", 1 / sum, 1),
                new TreeProbability("tree8", 1 / sum, 1),
                new TreeProbability("tree9", 1 / sum, 1),
                new TreeProbability("tree10", 1 / sum, 1),
                new TreeProbability("tree11", 1 / sum, 1),
                new TreeProbability("tree12", 1 / sum, 1),
                new TreeProbability("tree13", 1 / sum, 1),
                new TreeProbability("tree14", 1 / sum, 1),
                new TreeProbability("tree15", 1 / sum, 1),
                new TreeProbability("tree16", 1 / sum, 1),
                new TreeProbability("tree17", 1 / sum, 1),
                new TreeProbability("tree18", 1 / sum, 1),
                new TreeProbability("tree19", 1 / sum, 1),
                new TreeProbability("tree20", 1 / sum, 1),
                new TreeProbability("tree21", 1 / sum, 1),
                new TreeProbability("tree22", 1 / sum, 1),
                new TreeProbability("tree23", 1 / sum, 1),
                new TreeProbability("tree24", 1 / sum, 1),
                new TreeProbability("tree25", 1 / sum, 1),
                new TreeProbability("tree26", 1 / sum, 1),
                new TreeProbability("tree27", 1 / sum, 1),
                new TreeProbability("tree28", 1 / sum, 1),
                new TreeProbability("tree29", 1 / sum, 1),
                new TreeProbability("tree30", 1 / sum, 1),
                new TreeProbability("tree31", 1 / sum, 1),
                new TreeProbability("tree32", 1 / sum, 1),
                new TreeProbability("tree33", 1 / sum, 1),
                new TreeProbability("tree34", 1 / sum, 1),
                new TreeProbability("tree35", 1 / sum, 1),
                new TreeProbability("tree36", 1 / sum, 1),
                new TreeProbability("tree37", 1 / sum, 1),
                new TreeProbability("tree38", 1 / sum, 1),
                new TreeProbability("tree39", 1 / sum, 1),
                new TreeProbability("tree40", 1 / sum, 1),
                new TreeProbability("tree41", 1 / sum, 1),
                new TreeProbability("tree42", 1 / sum, 1),
                new TreeProbability("tree43", 1 / sum, 1),
                new TreeProbability("tree44", 1 / sum, 1),
                new TreeProbability("tree45", 1 / sum, 1),
                new TreeProbability("tree46", 1 / sum, 1),
                new TreeProbability("tree47", 1 / sum, 1),
                new TreeProbability("tree48", 1 / sum, 1),
                new TreeProbability("tree49", 1 / sum, 1),
                new TreeProbability("tree50", 1 / sum, 1),
                new TreeProbability("tree51", 1 / sum, 1),
                new TreeProbability("tree52", 1 / sum, 1),
                new TreeProbability("tree53", 1 / sum, 1),
                new TreeProbability("tree54", 1 / sum, 1),
                new TreeProbability("tree55", 1 / sum, 1),
                new TreeProbability("tree56", 1 / sum, 1),
                new TreeProbability("tree57", 1 / sum, 1),
                new TreeProbability("tree58", 1 / sum, 1),
                new TreeProbability("tree59", 1 / sum, 1),
                new TreeProbability("tree60", 1 / sum, 1),
                new TreeProbability("tree61", 1 / sum, 1),
                new TreeProbability("tree62", 1 / sum, 1),
                new TreeProbability("tree63", 1 / sum, 1),
                new TreeProbability("tree64", 1 / sum, 1),
                new TreeProbability("tree65", 1 / sum, 1),
                new TreeProbability("tree66", 1 / sum, 1),
                new TreeProbability("tree67", 1 / sum, 1),
                new TreeProbability("tree68", 1 / sum, 1),
                new TreeProbability("tree69", 1 / sum, 1),
                new TreeProbability("tree70", 1 / sum, 1),
                new TreeProbability("tree71", 1 / sum, 1),
                new TreeProbability("tree72", 1 / sum, 1),
                new TreeProbability("tree73", 1 / sum, 1),
                new TreeProbability("tree74", 1 / sum, 1),
                new TreeProbability("tree75", 1 / sum, 1),
                new TreeProbability("tree76", 1 / sum, 1),
                new TreeProbability("tree77", 1 / sum, 1),
                new TreeProbability("tree78", 1 / sum, 1),
                new TreeProbability("tree79", 1 / sum, 1),
                new TreeProbability("tree80", 1 / sum, 1),
                new TreeProbability("tree81", 1 / sum, 1),
                new TreeProbability("tree82", 1 / sum, 1),
                new TreeProbability("tree83", 1 / sum, 1),
                new TreeProbability("tree84", 1 / sum, 1),
                new TreeProbability("tree85", 1 / sum, 1),
                new TreeProbability("tree86", 1 / sum, 1),
                new TreeProbability("tree87", 1 / sum, 1),
                new TreeProbability("tree88", 1 / sum, 1),
                new TreeProbability("tree89", 1 / sum, 1),
                new TreeProbability("tree90", 1 / sum, 1),
                new TreeProbability("tree91", 1 / sum, 1),
                new TreeProbability("tree92", 1 / sum, 1),
                new TreeProbability("tree93", 1 / sum, 1),
                new TreeProbability("tree94", 50 / sum, 1),
                new TreeProbability("tree95", 50 / sum, 1),
                new TreeProbability("tree96", 100 / sum, 1),
                new TreeProbability("tree97", 100 / sum, 1),
                new TreeProbability("tree98", 100 / sum, 1),
                new TreeProbability("tree99", 100 / sum, 1),
                new TreeProbability("tree100", 100 / sum, 1),
            };
            var sut = new ProbabilityCalculator();

            var result = sut.Calculate(trees);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Calculate_101Trees_ShouldThrowException()
        {
            var trees = Enumerable.Range(0, 101)
                .Select(x => new Tree() { Name = "tree", Probability = 1 })
                .ToList();

            var sut = new ProbabilityCalculator();

            Action calculateAction = () => sut.Calculate(trees);

            calculateAction.Should()
                .Throw<Exception>()
                .WithMessage("Tree count must be lower or equal to 100.");
        }
    }
}
