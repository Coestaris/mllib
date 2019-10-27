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

        private static ConvolutionalNeuralNetwork LoadNetwork(string filename)
        {
            return JSONParser.Parse(filename);
        }

        private static ConvolutionalNeuralNetwork InitNetwork(Size size)
        {
            var network = new ConvolutionalNeuralNetwork();
            var layers = new List<CNNLayer>()
            {
                new InputLayer(size, false),
                new ConvolutionalLayer(2, 5, 2, 1),
                new ReLuLayer(),
                new SubsamplingLayer(2, 2, 0),
                new ConvolutionalLayer(2, 5, 2, 1),
                new ReLuLayer(),
                new SubsamplingLayer(2, 2, 0),
                new FullyConnectedLayer(10),
                new SoftmaxLayer(),
            };

            network.PushLayers(layers);
            return network;
        }

        public static void Main(string[] args)
        {
            var bitmap = new Bitmap("img.png");
            var volume = InputLayer.BitmapToVolume(bitmap, false);

            //var network = InitNetwork(bitmap.Size);
            var network = LoadNetwork("net.json");
            var result = network.ForwardPass(volume);

            Console.WriteLine(string.Join(", ",
                result.WeightsRaw.Select(p => p.ToString("F3"))));

            for (var i = 0; i < network.Layers.Count; i++)
            {
                var l = network.Layers[i];
                if(l.OutSize.Width == 1 && l.OutSize.Height == 0)
                    continue; //vector

                for(var d = 0; d < l.OutDepth; d++)
                    l.ToBitmap(d).Save($"{i}_{d}.png");
            }
        }
    }
}