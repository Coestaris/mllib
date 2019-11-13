using System;
using System.Collections.Generic;
using ml.AI.GA;

namespace Tests
{
    internal class Program
    {
        public static double FitnessFunc(Genome gen)
        {
            var x = gen.Genes[0];
            var y = gen.Genes[1];

            return -(x * x / 10.0 + y * y / 6.0) + 12.0;
        }

        public static void Main(string[] args)
        {
            var random = new Random();
            var population = new Population(50, i => new Genome(
                new List<double>
                {
                    random.Next() % 2 == 0 ? random.NextDouble() * 10 - 20 : random.NextDouble() * 10 + 10,
                    random.Next() % 2 == 0 ? random.NextDouble() * 10 - 20 : random.NextDouble() * 10 + 10,
                },
                FitnessFunc));

            Genome bestCreature = null;
            while (bestCreature == null || Math.Abs(bestCreature.Fitness - 12) > 1e-4)
            {
                bestCreature = population.BestCreature(false);
                Console.WriteLine("X: {0:F3}, Y : {1:F3}, Fitness: {2:F5}",
                    bestCreature.Genes[0],
                    bestCreature.Genes[1],
                    bestCreature.Fitness);

                population.Selection(false);
                population.Crossover( CrossoverAlgorithm.Blend);
                population.Mutate(0.2);
            }
        }
    }
}