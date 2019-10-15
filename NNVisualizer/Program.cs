using System;
using System.Drawing;
using ml.AI;
using OpenTK.Graphics;

namespace NNVisualizer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var win = new Window(800, 600, "NNVisualizer")
            {
                BackgroundColor = new Color4(.2f, .2f, .2f, 0)
            };

            var network = new NeuralNetwork(new []{ 2, 4, 2} );
            var random = new Random();
            network.Fill(
                (i, j) => random.NextDouble(),
                (i, j) => random.NextDouble(),
                (i, j) => 0);

            var teacher = new Teacher(1000, 1000, i =>
            {
                var input = new double[] {random.Next() % 2, random.Next() % 2};
                var result = ((int) input[0] ^ (int) input[1]) == 1;
                var expected = new double[] {result ? 1 : 0, result ? 0 : 1};
                return new TeacherTask(input, expected);
            });

            WindowHandler handler = new NNVisualizer(win, network);
            handler.Start();
        }
    }
}