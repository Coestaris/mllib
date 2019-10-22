using System;
using System.Linq;
using ml.AIMath;

namespace ml.AI.MBNN
{
    public class NeuralNetwork : INetwork
    {
        private readonly Matrix[] _Zs;

        private readonly Matrix[] _nablaW;
        private readonly Matrix[] _nablaB;
        internal readonly Matrix[] Activations;

        public readonly Matrix[] Biases;
        public readonly Matrix[] Weights;

        public readonly int LayersCount;
        public readonly int[] LayerSizes;

        public NeuralNetwork(int[] sizes)
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

        public void Print()
        {
            foreach (var str in Weights.Zip(Biases, (a, b) => new { a, b }))
            {
                str.a.Print();
                str.b.Print();
            }
        }

        public void FillRandom(Random random = null)
        {
            foreach (var matrix in Weights)
                matrix.FillRandom(random);
            foreach (var matrix in Biases)
                matrix.FillRandom(random);
        }

        public void FillGaussianRandom(GaussianRandom gaussianRandom = null)
        {
            foreach (var matrix in Weights)
                matrix.FillGaussianRandom(0, Math.Sqrt(2.0 / LayerSizes[0]), gaussianRandom);
            foreach (var matrix in Biases)
                matrix.FillGaussianRandom(0, 0.001, gaussianRandom);
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
            var dA = (Activations[LayersCount - 1] - new Matrix(expected)) *
                        ActivationFunctions.SigmoidD(_Zs[LayersCount - 1]);

            _nablaW[_nablaW.Length - 1] += dA.Dot(Activations[LayersCount - 2].Transpose());
            _nablaB[_nablaB.Length - 1] += dA;

            for (int i = 2; i < LayersCount; i++)
            {
                var dZ = ActivationFunctions.SigmoidD(_Zs[LayersCount - i]);

                dA = Weights[Weights.Length - i + 1].Transpose().Dot(dA) * dZ;

                _nablaW[Weights.Length - i] += dA.Dot(Activations[LayersCount - i - 1].Transpose());
                _nablaB[Weights.Length - i] += dA;
            }
        }

        public double LearningRate = 0.5;

        public void ApplyNudge(int count, int totalCount)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Weights[i] - (_nablaW[i] * (LearningRate / (double)count));
                _nablaW[i].Fill(0);

                Biases[i] = Biases[i] - (_nablaB[i] * (LearningRate / count));
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