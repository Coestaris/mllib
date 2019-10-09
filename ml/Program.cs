using System;
using ml.AI;

namespace ml
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var network = new NeuralNetwork(new[] {2, 2, 2});
            var random = new Random();
            network.Fill(
                (i, j) => random.NextDouble(),
                (i, j) => random.NextDouble(),
                (i, j) => 0);


            var teacher = new Teacher(10000, 100, i =>
            {
                var input = new double[] {random.Next() % 2, random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });

            network.Layers[0].Biases = new double[2] { 0, 0 };
            network.Layers[1].Biases = new double[2] { 0.35, 0.35 };
            network.Layers[0].Weights = new double[4] {.15, .20, .25, .30};

            network.Layers[2].Biases = new double[2] { 0.60, 0.60 };
            network.Layers[1].Weights = new double[4] {.40, .45, .50, .50};
            network.Print();

            teacher.Teach(network);
            Console.WriteLine();
            network.Print();
        }
    }
}