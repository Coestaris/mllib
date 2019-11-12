using System;
using System.Collections.Generic;
using ml.AIMath;

namespace ml.AI.GA
{
    public enum CrossoverAlgorithm
    {
        SinglePoint,
        TwoPoint,
        ThreePoint,

        Linear,
        Blend,
        SBC
    }

    public class Genome
    {
        public List<double> Genes;

        private static Random _random = new Random();
        private static GaussianRandom _gaussianRandom = new GaussianRandom(_random);

        public const double GaussianMean = .5;

        private Genome()
        {
            Genes= new List<double>();
        }

        public Genome(List<double> genes)
        {
            Genes = genes;
        }

        public static Genome[] Crossover(
            Genome parent1, Genome parent2,
            CrossoverAlgorithm algorithm, bool gaussian)
        {
            switch (algorithm)
            {
                case CrossoverAlgorithm.SinglePoint:
                    return CrossoverNPoint(parent1, parent2, gaussian, 1);
                case CrossoverAlgorithm.TwoPoint:
                    return CrossoverNPoint(parent1, parent2, gaussian, 2);
                case CrossoverAlgorithm.ThreePoint:
                    return CrossoverNPoint(parent1, parent2, gaussian, 3);

                case CrossoverAlgorithm.Linear:
                case CrossoverAlgorithm.Blend:
                case CrossoverAlgorithm.SBC:
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }
        }

        //returns random value in range [min, max]
        private static double Random(bool gaussian, double min, double max)
        {
            return Random(gaussian) * (max - min) + min;
        }

        //returns random value in range [0, 1]
        private static double Random(bool gaussian)
        {
            double rand;
            if (gaussian)
            {
                rand = _gaussianRandom.Next(GaussianMean);
                if (rand > 1) rand = 1;
                else if (rand < 0) rand = 0;
            }
            else
                rand = _random.NextDouble();

            return rand;
        }

        private static Genome[] CrossoverNPoint(Genome parent1, Genome parent2, bool gaussian, int pointsCount)
        {
            var len = parent1.Genes.Count;
            var points = new List<int>();
            for (var i = 0; i < pointsCount; i++)
                points.Add((int)Random(
                    gaussian,
                    i * len / (double) pointsCount + 1,
                    (i + 1) * len / (double) pointsCount - 1));

            points.Add(len);
            pointsCount++;

            Console.WriteLine(string.Join(", ", points));

            var child1 = new Genome();
            var child2 = new Genome();

            for (var i = 0; i < len; i++)
            {
                var pointIndex = 0;
                for(var j = 0; j < pointsCount - 1; j++)
                    if (i >= points[j] && i < points[j + 1])
                    {
                        pointIndex = j + 1;
                        break;
                    }

                if (pointIndex % 2 == 0)
                {
                    child1.Genes.Add(parent1.Genes[i]);
                    child2.Genes.Add(parent2.Genes[i]);
                }
                else
                {
                    child1.Genes.Add(parent2.Genes[i]);
                    child2.Genes.Add(parent1.Genes[i]);
                }
            }

            return new[]
            {
                child1,
                child2
            };
        }
    }
}