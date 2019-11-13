using System;
using System.Collections.Generic;
using System.Linq;
using MLLib.AIMath;

namespace MLLib.AI.OBNN
{
    public class ImprovedNeuralNetwork : NeuralNetwork
    {
        public readonly CostFunction CostFunction;
        public double RegularizationLambda = 1;

        public ImprovedNeuralNetwork(IReadOnlyList<int> layerSizes,
            CostFunction costFunction = null) : base(layerSizes)
        {
            if (costFunction == null) costFunction = new CrossEntropyCostFunction();
            CostFunction = costFunction;
        }

        public override void BackProp(double[] expected)
        {
            //output layer derivatives
            var outputLayer = Layers.Last();
            var prevLayer = Layers[Layers.Count - 2];

            for (int n = 0; n < outputLayer.Size; n++)
            {
                double delta = 0;
                delta = CostFunction.Delta(
                    !CostFunction.ActivationDerivative ? outputLayer._Zs[n] : outputLayer.Activations[n],
                    outputLayer.Activations[n],
                    expected[n]);

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

        public override double CalculateError(double[] expected)
        {
            if(expected == null)
                throw new ArgumentNullException(nameof(expected));

            if(expected.Length != Layers.Last().Size)
                throw new ArgumentException("Sizes of input and input layer doesn't match", nameof(expected));

            return CostFunction.GetCost(Layers.Last().Activations, expected);
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
                        prevLayer.Weights[p * currentLayer.Size + n] =
                            (1 - LearningRate * RegularizationLambda / totalCount) * prevLayer.Weights[p * currentLayer.Size + n] -
                            LearningRate * _weightNudges[l - 1][p * currentLayer.Size + n] / count;

                        _weightNudges[l - 1][p * currentLayer.Size + n] = 0;
                    }

                }
            }
        }
    }
}