using System;
using System.Collections.Generic;
using System.Linq;
using ml.AIMath;

namespace ml.AI.OBNN
{
    public class ObjectBasedNeuralNetwork : INetwork
    {
        public readonly List<NNLayer> Layers;

        public ObjectBasedNeuralNetwork(IReadOnlyList<int> layerSizes)
        {
            if(layerSizes == null)
                throw new ArgumentNullException(nameof(layerSizes));

            Layers = new List<NNLayer>();

            _biasNudges = new double[layerSizes.Count][];
            _weightNudges = new double[layerSizes.Count][];

            for(var i = 0; i < layerSizes.Count; i++)
            {
                Layers.Add(new NNLayer(layerSizes[i],
                    i != layerSizes.Count - 1 ? layerSizes[i + 1] : -1,
                    i != 0 ? layerSizes[i - 1] : 0));

                _biasNudges[i] = new double[layerSizes[i]];
                _weightNudges[i] = new double[layerSizes[i] * (i != layerSizes.Count - 1 ? layerSizes[i + 1] : 0)];
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

        public void ForwardPass(double[] input)
        {
            if(input == null)
                throw new ArgumentNullException(nameof(input));

            if(input.Length != Layers[0].Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(input));

            Layers[0].SetActivation(input);
            for(var i = 0; i < Layers.Count - 1; i++)
                Layers[i].ForwardPass(Layers[i + 1]);
        }

        public double CalculateError(double[] expected)
        {
            if(expected == null)
                throw new ArgumentNullException(nameof(expected));

            if(expected.Length != Layers.Last().Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(expected));

            return Layers.Last().CalculateError(expected);
        }

        public double LearningRate = 0.5;

        private double[][] _biasNudges;
        private double[][] _weightNudges;

        public void BackProp(double[] expected)
        {
            //output layer derivatives
            var outputLayer = Layers.Last();
            var prevLayer = Layers[Layers.Count - 2];

            for (int n = 0; n < outputLayer.Size; n++)
            {
                double dA = outputLayer.Activations[n] - expected[n];
                double dZ = ActivationFunctions.SigmoidDS(outputLayer.Activations[n]);
                for (int j = 0; j < prevLayer.Size; j++)
                    outputLayer.Derivatives[n].dW[j] = dA * dZ * prevLayer.Activations[j];

                outputLayer.Derivatives[n].dA = dA;
                outputLayer.Derivatives[n].dZ = dZ;
                outputLayer.Derivatives[n].dB = dA * dZ;
            }

            //other layers derivatives
            for (int l = Layers.Count - 2; l >= 1; l--)
            {
                var currentLayer = Layers[l];
                prevLayer = Layers[l - 1];
                var nextLayer = Layers[l + 1];

                //every neuron loop
                for (int n = 0; n < currentLayer.Size; n++)
                {
                    double dA = 0;
                    //next layer`s neurons loop
                    for (int j = 0; j < nextLayer.Size; j++)
                    {
                        double nextDA = nextLayer.Derivatives[j].dA;
                        double nextDZ = nextLayer.Derivatives[j].dZ;
                        dA += nextDZ * nextDA * currentLayer.Weights[n * nextLayer.Size + j];
                    }

                    double dZ = ActivationFunctions.SigmoidDS(currentLayer.Activations[n]);
                    if(prevLayer != null)
                        for (int j = 0; j < prevLayer.Size; j++)
                            currentLayer.Derivatives[n].dW[j] = dA * dZ * prevLayer.Activations[j];

                    currentLayer.Derivatives[n].dA = dA;
                    currentLayer.Derivatives[n].dZ = dZ;
                    currentLayer.Derivatives[n].dB = dA * dZ;
                }
            }

            //save the nudge
            for (int l = Layers.Count - 1; l >= 1; l--)
            {
                var currentLayer = Layers[l];
                prevLayer = Layers[l - 1];
                for (int n = 0; n < currentLayer.Size; n++)
                {
                    //bias
                    _biasNudges[l][n] += currentLayer.Derivatives[n].dB;

                    //weight
                    for (int p = 0; p < prevLayer.Size; p++)
                        _weightNudges[l - 1][p * currentLayer.Size + n] +=
                            currentLayer.Derivatives[n].dW[p];

                }
            }
        }

        public void ApplyNudge(int count)
        {
            //apply the nudge
            for (int l = Layers.Count - 1; l >= 1; l--)
            {
                var currentLayer = Layers[l];
                var prevLayer = Layers[l - 1];
                for (int n = 0; n < currentLayer.Size; n++)
                {
                    //bias
                    currentLayer.Biases[n] -= LearningRate * _biasNudges[l][n] / count;
                    _biasNudges[l][n] = 0;

                    //weight
                    for (int p = 0; p < prevLayer.Size; p++)
                    {
                        prevLayer.Weights[p * currentLayer.Size + n] -=
                            LearningRate * _weightNudges[l - 1][p * currentLayer.Size + n] / count;
                        _weightNudges[l - 1][p * currentLayer.Size + n] = 0;
                    }

                }
            }
        }

        public double[] Run(double[] input)
        {
            ForwardPass(input);
            return Layers.Last().Activations;
        }
    }
}