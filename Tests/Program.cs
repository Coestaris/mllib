using System;
using System.IO;
using System.Linq;
using ml.AI.RNN;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var rnn = new RecurrentNeuralNetwork(input, 100, 12, 0.1);
            rnn.Train(1000, 100, (i, loss) =>
            {
                Console.Write(rnn.Sample(input[0], 50));
                Console.WriteLine("=== Iteration {0}. Loss: {1}", i, loss);
                return false;
            });

            Console.WriteLine("====");
            Console.WriteLine("====");
            Console.WriteLine("====");

            while (true)
            {
                Console.WriteLine("Input some character: ");
                var c = Console.ReadLine();
                var ch = c[0];

                if (!rnn.Vocab.Contains(ch))
                {
                    Console.WriteLine("Vocab doesnt contain character {0}. Vocab: [{1}]",
                        ch, string.Join(",", rnn.Vocab.Select(p => $"'{p}'")));

                    continue;
                }

                char outCh;
                var probability = rnn.GetNextCharProbability(ch, out outCh);
                Console.WriteLine("{0:F3} : {1}", probability, outCh);
            }
        }
    }
}