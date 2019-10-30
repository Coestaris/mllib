using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HWDRecognizer;
using ml.AI;
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
            var dataset = new Dataset(
                "data/dataset.data",
                "data/datasetLabels.data",
                "data/test.data",
                "data/testLabels.data"
            );


            var testVolumes = dataset.TestImages.Select(p => new { volume = p.ToVolume(), label = p.Number }).ToList();
            var trainVolumes = dataset.DatasetImages.Select(p => new { volume = p.ToVolume(), label = p.Number }).ToList();

            var network = InitNetwork(new Size(
                testVolumes[0].volume.SX,
                testVolumes[0].volume.SY));

            var trainer = new Trainer(network, 0.4);
            for (int i = 0; i < 10; i++)
            {
                double loss = 0;
                foreach (var trainVolume in trainVolumes)
                {
                    loss += trainer.Train(trainVolume.volume, trainVolume.label);
                }

                Console.WriteLine("Epoch: {0}. Loss: {1:F5}", i, loss / trainVolumes.Count);
            }

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