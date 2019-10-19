using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ml.AIMath;

namespace ml.AI.OBNN
{
    public class MBNeuralNetwork : INetwork
    {
        internal Matrix[] Activations;
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
                matrix.FillGaussianRandom(Math.Sqrt(2.0 / LayerSizes[0]), 0);
            foreach (var matrix in Biases)
                matrix.FillGaussianRandom(0.001, 0);
        }

        public void Print()
        {
            foreach (var matrix in Weights.Zip(Biases, (m1, m2) => new { a = m1, b = m2 }))
            {
                matrix.a.Print();
                matrix.b.Print();
            }
        }

        public void ForwardPass(double[] input)
        {
            if(input.Length != LayerSizes[0])
                throw new ArgumentException();

            Activations[0] = new Matrix(input);
            for(var i = 0; i < LayersCount - 1; i++)
                Activations[i + 1] = ActivationFunctions.Sigmoid(Activations[i] * Weights[i] + Biases[i]);
        }

        public void BackProp(double[] expected)
        {
            throw new NotImplementedException();
        }

        public void ApplyNudge(int count)
        {
            throw new NotImplementedException();
        }

        public double CalculateError(double[] expected)
        {
            throw new NotImplementedException();
        }
    }
}