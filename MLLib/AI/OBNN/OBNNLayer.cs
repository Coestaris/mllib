using System;
using System.Linq;
using System.Text;
using ml.AIMath;

namespace ml.AI.OBNN
{
    public class NNLayer
    {
        private static Random _random = new Random((int)DateTime.Now.ToBinary());
        private static GaussianRandom _gaussianRandom = new GaussianRandom(_random);

        public int Size;
        public int NextLayerSize;
        public   readonly double[] Activations;
        public   readonly double[] Biases;
        public   readonly double[] Weights; //to right
        internal readonly double[] _Zs;
        internal readonly DerivativePack[] Derivatives;

        internal NNLayer(int size, int nextLayerSize, int prevLayerSize)
        {
            Size = size;
            NextLayerSize = nextLayerSize;

            Activations = new double[size];
            _Zs = new double[size];

            Biases = new double[size];

            if(nextLayerSize != -1)
                Weights = new double[size * nextLayerSize];

            Derivatives = new DerivativePack[size];
            for(int i = 0; i < size; i++)
                Derivatives[i] = new DerivativePack() {
                    dW = new double[prevLayerSize]
                };
        }

        internal void FillRandom(Random random)
        {
            if (random == null) random = _random;

            for (var i = 0; i < Biases.Length; i++)
                Biases[i] = random.NextDouble();

            if (Weights != null)
                for (var i = 0; i < Weights.Length; i++)
                    Weights[i] = random.NextDouble();
        }

        internal void FillGaussianRandom(int firstLayerSize, GaussianRandom random)
        {
            if (random == null) random = _gaussianRandom;

            for (var i = 0; i < Biases.Length; i++)
                Biases[i] = random.Next(0, 0.001);

            if (Weights!=null)
                for (var i = 0; i < Weights.Length; i++)
                    Weights[i] = random.Next(0, Math.Sqrt(2.0 / firstLayerSize));
        }

        public void Print(int index)
        {
            var sb = new StringBuilder();
            if (index == 0) sb.Append("Input layer.");
            else if (Weights == null) sb.Append("Output layer.");
            else sb.AppendFormat("Hidden layer #{0}.", index);

            sb.Append(" Activations: [");
            sb.Append(string.Join(", ", Activations.Select(p => p.ToString("0.0000"))));

            sb.Append("]. Biases: [");
            sb.Append(string.Join(", ", Biases.Select(p => p.ToString("0.0000"))));

            if (Weights != null)
            {
                sb.Append("]. Weights: [");
                sb.Append(string.Join(", ", Weights.Select(p => p.ToString("0.0000"))));

            }

            sb.Append("]");


            Console.WriteLine(sb.ToString());
        }

        public void SetActivation(double[] input)
        {
            Array.Copy(input, Activations, input.Length);
        }

        internal void ForwardPass(NNLayer nextLayer)
        {
            for (var next = 0; next < nextLayer.Size; next++)
            {
                var acc = nextLayer.Biases[next];
                for (var curr = 0; curr < Size; curr++)
                    acc += Activations[curr] * Weights[next + nextLayer.Size * curr];

                nextLayer._Zs[next] = acc;
                nextLayer.Activations[next] = ActivationFunctions.Sigmoid(acc);
            }
        }

        public double CalculateError(double[] expected)
        {
            double acc = 0;
            for (var i = 0; i < Size; i++)
                acc += Math.Pow(expected[i] - Activations[i], 2);
            return acc;
        }
    }
}