using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MLLib.AI.GA
{
    public class Population
    {
        public int Count;
        public List<Genome> Pop;
        public const int CrossoverRange = 5;

        private Thread[] _threads;

        public Population(IEnumerable<Genome> pop)
        {
            Pop = new List<Genome>(pop);
            for (var i = 0; i < Pop.Count; i++)
                Pop[i].CreatureID = i;
            Count = Pop.Count;
        }

        public Population(int count, Func<int, Genome> creatorFunc)
        {
            Pop = new List<Genome>();
            for (var i = 0; i < count; i++)
            {
                var gen = creatorFunc(i);
                gen.CreatureID = i;
                Pop.Add(gen);
            }

            Count = count;
        }

        public Genome BestCreature(bool min)
        {
            return GetSortedPop(min)[0];
        }

        public void EvaluateFitness()
        {
            foreach (var genome in Pop)
                genome.CalculateFitness();
        }

        public void MultiThreadEvaluateFitness(int threads)
        {
            var len = Pop.Count / threads;
            var off = Pop.Count % threads;
            if (off != 0) threads++;

            var genomes = Pop.ToArray().Split(len).Select(p => p.ToArray()).ToArray();

            _threads = new Thread[threads];
            for(var i = 0; i < threads; i++)
                _threads[i] = new Thread(AsyncFitnessFunc);

            for (var i = 0; i < threads; i++)
            {
                if(i == threads - 1)
                    _threads[i].Start(Pop.Skip(len * (threads - 1)).ToArray());
                else
                    _threads[i].Start(genomes[i]);
            }

            for (var i = 0; i < threads; i++)
                _threads[i].Join();
        }

        private static void AsyncFitnessFunc(object obj)
        {
            var genomes = (Genome[]) obj;
            foreach (var genome in genomes)
                genome.CalculateFitness();
        }

        public void Selection(bool minimize, int take = -1)
        {
            Pop = GetSortedPop(minimize).Take(take == -1 ? Count / 2 : take).ToList();
        }

        private List<Genome> GetSortedPop(bool min)
        {
            return min ? Pop.OrderBy(p => p.Fitness).ToList() : Pop.OrderByDescending(p => p.Fitness).ToList();
        }

        private int ClipCrossoverIndex(int i, bool min)
        {
            if (min) return i < 0 ? 0 : i;
            return i > Pop.Count - 1 ? Pop.Count - 1: i;
        }

        public void Crossover(CrossoverAlgorithm algorithm, bool gaussian = true)
        {
            var newPop = new List<Genome>();
            while (newPop.Count + Pop.Count < Count)
            {
                var minIndex = ClipCrossoverIndex(0, true);
                var maxIndex = ClipCrossoverIndex(Pop.Count, false);

                var i1 = (int)Genome.Random(gaussian, minIndex, maxIndex);
                var i2 = (int)Genome.Random(gaussian, minIndex, maxIndex);

                if (i1 == i2)
                {
                    i1 = ClipCrossoverIndex(i1 - 1, true);
                    i2 = ClipCrossoverIndex(i2 + 1, false);
                }

                var newGenomes = Genome.Crossover(Pop[i1], Pop[i2], algorithm, gaussian);

                if(newPop.Count < Count - Pop.Count)
                    newPop.Add(newGenomes[0]);
                if(newPop.Count < Count - Pop.Count)
                    newPop.Add(newGenomes[1]);
            }

            Pop.AddRange(newPop);

            if(Pop.Count != Count)
                throw new Exception();
        }

        public void Mutate(double mutationRate, bool gaussian = false)
        {
            foreach (var genome in Pop)
                genome.Mutate(mutationRate, gaussian);
        }

        public double AverageFitness()
        {
            return Pop.Sum(p => p.Fitness) / Pop.Count;
        }

        public List<object> GetStates()
        {
            return Pop.Select(p => p.State).ToList();
        }
    }
}