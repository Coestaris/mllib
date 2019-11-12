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

            for (var i = 0; i < 10; i++)
            {
                gen1.Genes.Add(i);
                gen2.Genes.Add(10 - i);
            }

            var children = Genome.Crossover(gen1, gen2, CrossoverAlgorithm.Linear, true);
            Console.WriteLine($"Parent1: [{string.Join(", ", gen1.Genes)}]");
            Console.WriteLine($"Parent2: [{string.Join(", ", gen2.Genes)}]");

            Console.WriteLine($"Child1:  [{string.Join(", ", children[0].Genes)}]");
            Console.WriteLine($"Child2:  [{string.Join(", ", children[1].Genes)}]");

            for (var i = 0; i < 10; i++)
            {
                children[1].Mutate(.1, true);
                Console.WriteLine($"Child2:  [{string.Join(", ", children[1].Genes)}]");
            }


        }
    }
}