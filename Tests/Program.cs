using System;
using ml.AI;
using ml.AI.MBNN;
using ml.AI.OBNN;
using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            var teacher = new Teacher(1000, 1, i =>
            {
                var input = new double[] {_random.Next() % 2, _random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });

            var network = new MBNeuralNetwork(new[] {2, 5, 2});

            network.FillGaussianRandom();
            network.Print();

            var epochCount = 10;
            for (int i = 0; i < epochCount; i++)
            {
                teacher.Teach(network);
                Console.WriteLine("Epoch {0}: {1}", i, teacher.Error);
                teacher.ResetError();
            }
            Console.WriteLine("After");
            network.Print();

            Console.WriteLine(string.Join(", ", network.ForwardPass(new double[] {1, 1})));
            Console.WriteLine(string.Join(", ", network.ForwardPass(new double[] {1, 0})));
            Console.WriteLine(string.Join(", ", network.ForwardPass(new double[] {0, 1})));
            Console.WriteLine(string.Join(", ", network.ForwardPass(new double[] {0, 0})));
        }
    }
}