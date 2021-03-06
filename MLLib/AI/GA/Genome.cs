using System;
using System.Collections.Generic;
using MLLib.AIMath;

namespace MLLib.AI.GA
{
    public class Genome : ICloneable
    {
        public int CreatureID;
        public List<double> Genes;
        public double Fitness;
        public ICreature Creature;

        private bool _isCreature;
        public object State;

        private Func<Genome, double> _fitnessFunc;

        private static Random _random = new Random();
        private static GaussianRandom _gaussianRandom = new GaussianRandom(_random);

        public const double GaussianMean = .5;

        public const double LinearCrossoverMin = .4;
        public const double LinearCrossoverMax = .6;

        public const double BlendCrossoverMin = .45;
        public const double BlendCrossoverMax = .55;

        public const double CrossoverSwapChance = 0.6;

        private Genome(Genome genome)
        {
            Genes = new List<double>();
            _isCreature = genome._isCreature;
            _fitnessFunc = genome._fitnessFunc;

            if (_isCreature)
                Creature = genome.Creature.CreatureChild();
        }

        public Genome(List<double> genes, ICreature creature)
        {
            Genes = genes;
            _isCreature = true;
            Creature = creature;
        }

        public Genome(List<double> genes, Func<Genome, double> fitnessFunc)
        {
            Genes = genes;
            _fitnessFunc = fitnessFunc ?? (genome => 0);
        }


        public object Clone()
        {
            return new Genome(this);
        }

        internal void CalculateFitness()
        {
            if (_isCreature)
            {
                var time = 0;
                Creature.Reset();
                Creature.Update(this);

                while (Creature.Step(time++)) { }

                State = Creature.GetState();
                Fitness = Creature.GetFitness();
            }
            else Fitness = _fitnessFunc(this);
        }

        internal void Mutate(double mutationRate, bool gaussian)
        {
            for (var i = 0; i < Genes.Count; i++)
                Genes[i] += Random(gaussian) * (mutationRate * 2) - mutationRate;
        }

        internal static Genome[] Crossover(
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
                    return CrossoverLinear(parent1, parent2, gaussian);
                case CrossoverAlgorithm.Blend:
                    return CrossoverBlend(parent1, parent2, gaussian);
                case CrossoverAlgorithm.RandomSwap:
                    return CrossoverRandomSwap(parent1, parent2, gaussian);

                case CrossoverAlgorithm.SBC:
                default:
                    throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
            }
        }

        //returns random value in range [min, max]
        internal static double Random(bool gaussian, double min, double max)
        {
            return Random(gaussian) * (max - min) + min;
        }

        //returns random value in range [0, 1]
        internal static double Random(bool gaussian)
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

        private static Genome[] CrossoverRandomSwap(Genome parent1, Genome parent2, bool gaussian)
        {
            var len = parent1.Genes.Count;
            var child1 = new Genome(parent1);
            var child2 = new Genome(parent1);

            for (var i = 0; i < len; i++)
            {
                var p1 = parent1.Genes[i];
                var p2 = parent2.Genes[i];
                if (Random(gaussian) > CrossoverSwapChance)
                {
                    child1.Genes.Add(p2);
                    child2.Genes.Add(p1);
                }
                else
                {
                    child1.Genes.Add(p1);
                    child2.Genes.Add(p2);
                }
            }

            return new[]
            {
                child1,
                child2
            };
        }

        private static Genome[] CrossoverSBC(Genome parent1, Genome parent2, bool gaussian)
        {
            var len = parent1.Genes.Count;
            var child1 = new Genome(parent1);
            var child2 = new Genome(parent1);

            for (var i = 0; i < len; i++)
            {
                var p1 = parent1.Genes[i];
                var p2 = parent2.Genes[i];
                var u = Random(gaussian);
                //todo:::
                child1.Genes.Add(0);
                child2.Genes.Add(0);
            }

            return new[]
            {
                child1,
                child2
            };
        }


        private static Genome[] CrossoverBlend(Genome parent1, Genome parent2, bool gaussian)
        {
            var len = parent1.Genes.Count;
            var child1 = new Genome(parent1);
            var child2 = new Genome(parent1);

            for (var i = 0; i < len; i++)
            {
                var p1 = parent1.Genes[i];
                var p2 = parent2.Genes[i];
                var a = Random(gaussian, BlendCrossoverMin, BlendCrossoverMax);

                child1.Genes.Add(Random(gaussian, p1 - a * (p2 - p1), p2 + a * (p2 - p1)));
                child2.Genes.Add(Random(gaussian, p1 - a * (p2 - p1), p2 + a * (p2 - p1)));
            }

            return new[]
            {
                child1,
                child2
            };
        }

        private static Genome[] CrossoverLinear(Genome parent1, Genome parent2, bool gaussian)
        {
            var len = parent1.Genes.Count;
            var child1 = new Genome(parent1);
            var child2 = new Genome(parent1);
            var a = Random(gaussian, LinearCrossoverMin, LinearCrossoverMax);
            //var a = 0.6;

            for (var i = 0; i < len; i++)
            {
                var p1 = parent1.Genes[i];
                var p2 = parent2.Genes[i];

                child1.Genes.Add(a * p1 + (1 - a) * p2);
                child2.Genes.Add(a * p2 + (1 - a) * p1);
            }

            return new[]
            {
                child1,
                child2
            };
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

            var child1 = new Genome(parent1);
            var child2 = new Genome(parent1);

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