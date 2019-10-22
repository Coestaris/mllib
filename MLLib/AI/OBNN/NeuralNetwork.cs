using System;
using System.Collections.Generic;
using System.Linq;
using ml.AIMath;

namespace ml.AI.OBNN
{
    public class NeuralNetwork : Network
    {
        public readonly List<NNLayer> Layers;
        public double LearningRate = 0.5;

        protected double[][] _biasNudges;
        protected double[][] _weightNudges;

        public NeuralNetwork(IReadOnlyList<int> layerSizes)
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

        public override void Print()
        {
            for(var i = 0; i < Layers.Count; i++)
                Layers[i].Print(i);
        }

        public override void FillRandom(Random random = null)
        {
            foreach (var layer in Layers)
                layer.FillRandom(random);
        }

        public override void FillGaussianRandom(GaussianRandom gaussianRandom = null)
        {
            foreach (var layer in Layers)
                layer.FillGaussianRandom(Layers[0].Size, gaussianRandom);
        }

        public override double[] ForwardPass(double[] input)
        {
            if(input == null)
                throw new ArgumentNullException(nameof(input));

            if(input.Length != Layers[0].Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(input));

            Layers[0].SetActivation(input);
            for(var i = 0; i < Layers.Count - 1; i++)
                Layers[i].ForwardPass(Layers[i + 1]);

            return Layers[Layers.Count - 1].Activations;
        }

        public override void BackProp(double[] expected)
        {
            //output layer derivatives
            var outputLayer = Layers.Last();
            var prevLayer = Layers[Layers.Count - 2];

            for (int n = 0; n < outputLayer.Size; n++)
            {
                double dA = outputLayer.Activations[n] - expected[n];
                double dZ = ActivationFunctions.SigmoidDS(outputLayer.Activations[n]);
                double delta = dA * dZ;

                for (int j = 0; j < prevLayer.Size; j++)
                    outputLayer.Derivatives[n].dW[j] = delta * prevLayer.Activations[j];

                outputLayer.Derivatives[n].Delta = delta;
                outputLayer.Derivatives[n].dB = delta;
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
                        double nextDelta = nextLayer.Derivatives[j].Delta;
                        dA += nextDelta * currentLayer.Weights[n * nextLayer.Size + j];
                    }

                    double dZ = ActivationFunctions.SigmoidDS(currentLayer.Activations[n]);
                    double delta = dA * dZ;

                    if(prevLayer != null)
                        for (int j = 0; j < prevLayer.Size; j++)
                            currentLayer.Derivatives[n].dW[j] = delta * prevLayer.Activations[j];

                    currentLayer.Derivatives[n].Delta = delta;
                    currentLayer.Derivatives[n].dB = delta;
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

        public override void ApplyNudge(int count, int totalCount)
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

        public override double CalculateError(double[] expected)
        {
            if(expected == null)
                throw new ArgumentNullException(nameof(expected));

            if(expected.Length != Layers.Last().Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(expected));

            return Layers.Last().CalculateError(expected);
        }
    }
}