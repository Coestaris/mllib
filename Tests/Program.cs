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
            var bitmap = new Bitmap("img.png");
            var volume = InputLayer.BitmapToVolume(bitmap, false);

            var network = new ConvolutionalNeuralNetwork();
            var layer1 = new InputLayer(bitmap.Size, true);
            var layer2 = new ConvolutionalLayer(3, 5, new Size(2, 2), new Size(1, 1));

            network.PushLayer(layer1);
            network.PushLayer(layer2);

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