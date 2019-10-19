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
/*            var teacher = new Teacher(10000, 100, i =>
            {
                var input = new double[] {_random.Next() % 2, _random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });*/

            var network1 = new MBNeuralNetwork(new[] {2, 3, 2});
            var network2 = new OBNeuralNetwork(new [] {2, 3, 2});

            network2.Fill();
/*            network.FillGaussianRandom();
            network.Print();
            teacher.BatchCallback = i =>
            {
                //Console.WriteLine(teacher.Error);
                teacher.ResetError();
            };
            teacher.Teach(network);
            Console.WriteLine("After");
            network.Print();*/

        }
    }
}