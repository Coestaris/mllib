using System;
using System.Drawing;
using ml.AI;
using ml.AI.OBNN;
using OpenTK.Graphics;
using WindowHandler;

namespace XORCalculator
{
    internal class Program
    {
        private static Random _random = new Random();
        private static ObjectBasedNeuralNetwork _network;

        public static void Reset()
        {
            _network.Fill(
                (i, j) => 3 - _random.NextDouble() * 6,
                (i, j) => 3 - _random.NextDouble() * 6,
                (i, j) => 0);
        }

        public static void Main(string[] args)
        {
            var win = new Window(1000, 700, "XORCalculator")
            {
                BackgroundColor = new Color4(94f / 255f, 91f / 255f, 102f / 255f, 0)
            };

            _network = new ObjectBasedNeuralNetwork(new []{ 2, 5, 2, 5, 2} );
            var teacher = new Teacher(10000, 100, i =>
            {
                var input = new double[] {_random.Next() % 2, _random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });

            var handler = new NNVisualizer(win, _network, teacher, Reset);
            handler.OnStart();
        }
    }
}