using System;
using System.Collections.Generic;
using System.Linq;
using ml.AIMath;

namespace ml.AI.MBNN
{
    public class MBNeuralNetwork : INetwork
    {
        internal Matrix[] Activations;
        private Matrix[] _Zs;

        private Matrix[] _nablaW;
        private Matrix[] _nablaB;

        internal Matrix[] Biases;
        internal Matrix[] Weights;

        public int LayersCount;
        public int[] LayerSizes;


        public MBNeuralNetwork(int[] sizes)
        {
            Biases = new Matrix[sizes.Length - 1];
            for (int i = 1; i < sizes.Length; i++)
                Biases[i - 1] = new Matrix(sizes[i], 1);

            Weights = new Matrix[sizes.Length - 1];
            for (int i = 0; i < sizes.Length - 1; i++)
                Weights[i] = new Matrix(sizes[i + 1], sizes[i]);

            LayersCount = sizes.Length;
            LayerSizes = sizes;

            Activations = new Matrix[LayersCount];
            _Zs = new Matrix[LayersCount];

            _nablaW = new Matrix[Weights.Length];
            _nablaB = new Matrix[Biases.Length];
            for (int i = 0; i < _nablaW.Length; i++)
            {
                _nablaW[i] = new Matrix(Weights[i]);
                _nablaB[i] = new Matrix(Biases[i]);
            }
        }

        public void FillRandom()
        {
            foreach (var matrix in Weights)
                matrix.FillRandom();
            foreach (var matrix in Biases)
                matrix.FillRandom();
        }

        public void FillGaussianRandom()
        {
            foreach (var matrix in Weights)
                matrix.FillGaussianRandom(0, Math.Sqrt(2.0 / LayerSizes[0]));
            foreach (var matrix in Biases)
                matrix.FillGaussianRandom(0, 0.001);
        }

        public void Print()
        {
            foreach (var str in Weights.Zip(Biases, (a, b) => new { a, b }))
            {
                str.a.Print();
                str.b.Print();
            }
        }

        public double[] ForwardPass(double[] input)
        {
            if(input.Length != LayerSizes[0])
                throw new ArgumentException();

            Activations[0] = new Matrix(input);
            for (var i = 0; i < LayersCount - 1; i++)
            {
                _Zs[i + 1] = Weights[i].Dot(Activations[i]) + Biases[i];
                Activations[i + 1] = ActivationFunctions.Sigmoid(_Zs[i + 1]);
            }

            return Activations[LayersCount - 1].ToArray();
        }

        public void PrintActivations()
        {
            foreach (var activation in Activations)
            {
                activation.Print();
            }
        }

        public void BackProp(double[] expected)
        {
            var delta = (Activations[LayersCount - 1] - new Matrix(expected)) *
                        ActivationFunctions.SigmoidD(_Zs[LayersCount - 1]);

            _nablaW[_nablaW.Length - 1] += delta.Dot(Activations[LayersCount - 2].Transpose());
            _nablaB[_nablaB.Length - 1] += delta;

            for (int i = LayersCount - 1; i >= 2; i--)
            {
                var z = _Zs[i - 1];
                var dZ = ActivationFunctions.SigmoidD(z);

                delta = Weights[i - 1].Transpose().Dot(delta) * dZ;

                _nablaW[i - 2] += delta.Dot(Activations[i - 1].Transpose());
                _nablaB[i - 2] += delta;
            }
        }

        public double LearningRate = 1;

        public void ApplyNudge(int count)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Weights[i] - _nablaW[i] * (LearningRate / count);
                _nablaW[i].Fill(0);

                Biases[i] = Biases[i] - _nablaB[i] * (LearningRate / count);
                _nablaB[i].Fill(0);
            }
        }

        public double CalculateError(double[] expected)
        {
            var diff = new Matrix(expected) - Activations[LayersCount - 1];
            return (diff * diff).ToArray().Sum();
        }
    }
}