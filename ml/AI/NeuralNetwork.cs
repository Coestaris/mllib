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

        private double LearningRate = 0.5;

        struct DerivativePack
        {
            public double   dA;
            public double   dZ;
            public double[] dW;
            public double   dB;

            public DerivativePack(double dA, double dZ, double[] dW, double dB)
            {
                this.dA = dA;
                this.dZ = dZ;
                this.dW = dW;
                this.dB = dB;
            }

        }

        internal double BackProp(double[] expected, double error)
        {
            var derivativePacks = new List<DerivativePack>[Layers.Count];

            //output layer derivatives
            var outputLayer = Layers.Last();
            var prevLayer = Layers[Layers.Count - 2];
            derivativePacks[Layers.Count - 1] = new List<DerivativePack>();

            for (int i = 0; i < outputLayer.Size; i++)
            {
                double dA = outputLayer.Activations[i] - expected[i];
                double dZ = NNLayer.DSigmoid(outputLayer.Activations[i]);
                double[] dW = new double[outputLayer.Size];
                for (int j = 0; j < prevLayer.Size; j++)
                    dW[j] = dA * dZ * prevLayer.Activations[j];

                double dB = dA * dZ;
                derivativePacks[Layers.Count - 1].Add(new DerivativePack(
                    dA, dZ, dW, dB));
            }

            //other layers derivatives
            for (int l = Layers.Count - 2; l >= 0; l--)
            {
                derivativePacks[l] = new List<DerivativePack>();
                var currentLayer = Layers[l];
                prevLayer = l >= 1 ? Layers[l - 1] : null;
                var nextLayer = Layers[l + 1];

                //every neuron loop
                for (int i = 0; i < currentLayer.Size; i++)
                {
                    double dA = 0;
                    //next layer`s neurons loop
                    for (int j = 0; j < nextLayer.Size; j++)
                    {
                        double nextDA = derivativePacks[l + 1][j].dA;
                        double nextDZ = derivativePacks[l + 1][j].dZ;
                        dA += nextDZ * nextDA * currentLayer.Weights[j * nextLayer.Size + i];
                    }

                    double dZ = NNLayer.DSigmoid(outputLayer.Activations[i]);
                    double[] dW = new double[outputLayer.Size];
                    if(prevLayer != null)
                        for (int j = 0; j < prevLayer.Size; j++)
                            dW[j] = dA * dZ * prevLayer.Activations[j];

                    double dB = dA * dZ;
                    derivativePacks[l].Add(new DerivativePack(
                        dA, dZ, dW, dB));
                }
            }

            return 0;
        }
    }
}