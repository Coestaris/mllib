using System;
using ml.AI;

namespace ml
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var network = new NeuralNetwork(new[] {2, 3, 2});
            var random = new Random();
            network.Fill(
                (i, j) => random.NextDouble(),
                (i, j) => random.NextDouble(),
                (i, j) => 0);

            network.Print();

            var teacher = new Teacher(10000, 100, i =>
            {
                var input = new double[] {random.Next() % 2, random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });


            teacher.Teach(network);
            Console.WriteLine();
            network.Print();
        }
    }
}