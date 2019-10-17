using System;
using ml.AI;

namespace ml
{
    internal class Program
    {
        public static void DoTest(double a, double b, NeuralNetwork network)
        {
            Console.WriteLine("[{0}]", string.Join(",",
                network.Run(new double[] {a, b})));
        }

        public static void Main(string[] args)
        {
            var network = new NeuralNetwork(new[] {2, 3, 2});
            var random = new Random();
            network.Fill(
                (i, j) => random.NextDouble(),
                (i, j) => random.NextDouble(),
                (i, j) => 0);


            var teacher = new Teacher(2300 * 40, 10, i =>
            {
                var input = new double[] {random.Next() % 2, random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });

            network.Print();

            teacher.Teach(network);
            network.Print();

            DoTest(1, 1, network);
            DoTest(1, 0, network);
            DoTest(0, 1, network);
            DoTest(0, 0, network);
        }
    }
}