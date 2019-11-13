using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MLLib.AI.CNN.Layers;

namespace MLLib.AI.CNN
{
    public class ConvolutionalNeuralNetwork
    {
        public List<CNNLayer> Layers { internal set; get; }
        public InputLayer InputLayer;
        public OutputLayer OutputLayer;

        public ConvolutionalNeuralNetwork()
        {
            Layers = new List<CNNLayer>();
        }

        private void PushLayer(CNNLayer layer)
        {
            if (Layers.Count != 0)
            {
                layer.PrevLayer = Layers.Last();
                layer.InSize = Layers.Last().OutSize;
                layer.InDepth = Layers.Last().OutDepth;
                Layers.Last().NextLayer = layer;

                if(layer is InputLayer)
                    throw new ArgumentException("You can't use more than 1 input layer");

                if(layer is OutputLayer)
                {
                    if(OutputLayer != null)
                        throw new ArgumentException("You can't use more than 1 output layer");
                    OutputLayer = (OutputLayer)layer;
                }
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

        public double BackwardPass(int correctIndex)
        {
            var loss = OutputLayer.BackwardPassLoss(correctIndex);
            for(var i = Layers.Count - 2; i >= 0; i--)
                Layers[i].BackwardPass();
            return loss;
        }

        public Volume ForwardPass(Volume volume)
        {
            var v = InputLayer.ForwardPass(volume);
            for (var i = 1; i < Layers.Count; i++)
            {
                Layers[i].InVolume = v;
                v = Layers[i].ForwardPass(v);
            }

            return v;
        }

        public void PushLayers(IEnumerable<CNNLayer> layers)
        {
            foreach (var layer in layers)
                PushLayer(layer);

            if(!(layers.Last() is OutputLayer))
                throw new ArgumentException("Last layer should be Output layer");
        }
    }
}