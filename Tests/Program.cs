using System;
using ml.AI.CNN;

namespace Tests
{
    internal class Program
    {
        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            var network = new ConvolutionalNeuralNetwork();
            network.Layers.Add(new ConvolutionalLayer());
        }
    }
}