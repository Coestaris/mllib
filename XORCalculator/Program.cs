using System;
using System.Drawing;
using ml.AI;
using OpenTK.Graphics;
using WindowHandler;

namespace XORCalculator
{
    internal class Program
    {
        private static Random _random = new Random();
        private static NeuralNetwork _network;

        public static void Reset()
        {
            _network.Fill(
                (i, j) => 1 - _random.NextDouble() * 2,
                (i, j) => 1 - _random.NextDouble() * 2,
                (i, j) => 0);
        }

        public static void Main(string[] args)
        {
            var win = new Window(1000, 800, "XORCalculator")
            {
                BackgroundColor = new Color4(.2f, .2f, .2f, 0)
            };

            _network = new NeuralNetwork(new []{ 2, 5, 5, 2} );
            var teacher = new Teacher(1000, 1000, i =>
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