using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ml.AI.CNN;
using ml.AI.CNN.Layers;
using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            var bitmap = new Bitmap("img1.png");
            var volume = InputLayer.BitmapToVolume(bitmap, false);

            var network = new ConvolutionalNeuralNetwork();

            var layers = new List<CNNLayer>()
            {
                new InputLayer(bitmap.Size, false),
                new ConvolutionalLayer(2, 5, 2, 1),
                new ReLuLayer(),
                new SubsamplingLayer(2, 2, 0),
                new ConvolutionalLayer(2, 5, 2, 1),
                new ReLuLayer(),
                new SubsamplingLayer(2, 2, 0),
            };

            network.PushLayers(layers);
            network.ForwardPass(volume);

            for (var i = 0; i < network.Layers.Count; i++)
            {
                var l = network.Layers[i];
                for(var d = 0; d < l.OutDepth; d++)
                    l.ToBitmap(d).Save($"{i}_{d}.png");
            }
        }
    }
}