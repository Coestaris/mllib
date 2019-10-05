using System;
using System.Collections.Generic;
using System.Linq;

namespace ml.AI
{
    public class NeuralNetwork
    {
        internal readonly List<NNLayer> Layers;

        public NeuralNetwork(IReadOnlyList<int> layerSizes)
        {
            if(layerSizes == null)
                throw new ArgumentNullException(nameof(layerSizes));

            Layers = new List<NNLayer>();
            for(var i = 0; i < layerSizes.Count; i++)
            {
                Layers.Add(new NNLayer(layerSizes[i],
                    i != layerSizes.Count - 1 ? layerSizes[i + 1] : -1));
            }
        }

        //Fill random
        public void Fill()
        {
            foreach (var layer in Layers) layer.Fill();
        }

        //Fill random and set bias
        public void Fill(double bias)
        {
            foreach (var layer in Layers) layer.Fill(bias);
        }

        //Fill random and set bias with weight
        public void Fill(double bias, double weight)
        {
            foreach (var layer in Layers) layer.Fill(bias, weight);
        }

        //Fill random and set bias, activation with weight
        public void Fill(double bias, double weight, double activation)
        {
            foreach (var layer in Layers) layer.Fill(bias, weight, activation);
        }

        //Fill with custom function
        public void Fill(Func<int, int, double> bias, Func<int, int, double> weight, Func<int, int, double> activation)
        {
            if(bias == null) throw new ArgumentNullException(nameof(bias));
            if(weight == null) throw new ArgumentNullException(nameof(weight));
            if(activation == null) throw new ArgumentNullException(nameof(activation));

            for(var i = 0; i < Layers.Count; i++)
                Layers[i].Fill(bias, weight, activation, i);
        }

        public void Print()
        {
            for(var i = 0; i < Layers.Count; i++)
                Layers[i].Print(i);
        }

        internal void ForwardPass(double[] input)
        {
            if(input == null)
                throw new ArgumentNullException(nameof(input));

            if(input.Length != Layers[0].Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(input));

            Layers[0].SetActivation(input);
            for(var i = 0; i < Layers.Count - 1; i++)
                Layers[i].ForwardPass(Layers[i + 1]);
        }

        internal double CalculateError(double[] expected)
        {
            if(expected == null)
                throw new ArgumentNullException(nameof(expected));

            if(expected.Length != Layers.Last().Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(expected));

            return Layers.Last().CalculateError(expected);
        }

        internal double BackProp(double error)
        {
            
        }
    }
}