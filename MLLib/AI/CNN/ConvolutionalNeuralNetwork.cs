using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ml.AI.CNN.Layers;

namespace ml.AI.CNN
{
    public class ConvolutionalNeuralNetwork
    {
        public List<CNNLayer> Layers { internal set; get; }
        public InputLayer InputLayer;

        public ConvolutionalNeuralNetwork()
        {
            Layers = new List<CNNLayer>();
        }

        public void PushLayer(CNNLayer layer)
        {
            if (Layers.Count != 0)
            {
                layer.PrevLayer = Layers.Last();
                layer.InSize = Layers.Last().OutSize;
                layer.InDepth = Layers.Last().OutDepth;
                Layers.Last().NextLayer = layer;

                if(layer is InputLayer)
                    throw new ArgumentException("You can't use more than 1 input layer");
            }
            else
            {
                if(!(layer is InputLayer))
                    throw new ArgumentException("First network layer should be " + nameof(InputLayer));
                InputLayer = (InputLayer)layer;
            }

            Layers.Add(layer);
            layer.Setup();
        }

        public void ForwardPass(Volume volume)
        {
            var v = InputLayer.ForwardPass(volume);
            for (var i = 1; i < Layers.Count; i++)
            {
                Layers[i].InVolume = v.Clone();
                v = Layers[i].ForwardPass(v);
            }
        }

        public void PushLayers(IEnumerable<CNNLayer> layers)
        {
            foreach (var layer in layers)
                PushLayer(layer);
        }
    }
}