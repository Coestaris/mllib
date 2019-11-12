using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ml.AI.GA;
using ml.AI.RNN;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var gen1 = new Genome(new List<double>());
            var gen2 = new Genome(new List<double>());

            for (var i = 0; i < 40; i++)
            {
                gen1.Genes.Add(1);
                gen2.Genes.Add(0);
            }

            Genome[] childs = null;

            for(int i = 0; i < 30; i++)
                childs = Genome.Crossover(gen1, gen2, CrossoverAlgorithm.ThreePoint, true);

            Console.WriteLine($"Parent1: [{string.Join(", ", gen1.Genes)}]");
            Console.WriteLine($"Parent2: [{string.Join(", ", gen2.Genes)}]");

            Console.WriteLine($"Child1:  [{string.Join(", ", childs[0].Genes)}]");
            Console.WriteLine($"Child2:  [{string.Join(", ", childs[1].Genes)}]");
        }
    }
}