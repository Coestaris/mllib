using System;
using System.Linq;
using System.Text;

namespace ml.AI
{
    internal  class NNLayer
    {
        private Random _random = new Random((int)DateTime.Now.ToBinary());

        public int Size;
        public int NextLayerSize;

        public double[] Activations;
        public double[] Biases;

        public double[] Weights; //to right

        public NNLayer(int size, int nextLayerSize)
        {
            Size = size;
            NextLayerSize = nextLayerSize;

            Activations = new double[size];
            Biases = new double[size];

            if(nextLayerSize != -1)
                Weights = new double[size * nextLayerSize];
        }

        public void Fill()
        {
            for (var i = 0; i < Activations.Length; i++) Activations[i] = _random.NextDouble();
            for (var i = 0; i < Biases.Length; i++) Biases[i] = _random.NextDouble();
            if (Weights!=null)
                for (var i = 0; i < Weights.Length; i++) Weights[i] = _random.NextDouble();
        }

        public void Fill(double bias)
        {
            for (var i = 0; i < Activations.Length; i++) Activations[i] = _random.NextDouble();
            for (var i = 0; i < Biases.Length; i++) Biases[i] = bias;
            if (Weights!=null)
                for (var i = 0; i < Weights.Length; i++) Weights[i] = _random.NextDouble();
        }

        public void Fill(double bias, double weight)
        {
            for (var i = 0; i < Activations.Length; i++) Activations[i] = _random.NextDouble();
            for (var i = 0; i < Biases.Length; i++) Biases[i] = bias;
            if (Weights!=null)
                for (var i = 0; i < Weights.Length; i++) Weights[i] = weight;
        }

        public void Fill(double bias, double weight, double activation)
        {
            for (var i = 0; i < Activations.Length; i++) Activations[i] = activation;
            for (var i = 0; i < Biases.Length; i++) Biases[i] = bias;
            if (Weights!=null)
                for (var i = 0; i < Weights.Length; i++) Weights[i] = weight;
        }

        public void Fill(Func<int, int, double> bias, Func<int, int, double> weight, Func<int, int, double> activation,
            int index)
        {
            for (var i = 0; i < Activations.Length; i++) Activations[i] = activation(index, i);
            for (var i = 0; i < Biases.Length; i++) Biases[i] =  bias(index, i);
            if (Weights != null)
                for (var i = 0; i < Weights.Length; i++) Weights[i] = weight(index, i);
        }

        public void Print(int index)
        {
            var sb = new StringBuilder();
            if (index == 0) sb.Append("Input layer.");
            else if (Weights == null) sb.Append("Output layer.");
            else sb.AppendFormat("Hidden layer #{0}.", index);

            sb.Append(" Activations: [");
            sb.Append(string.Join(", ", Activations.Select(p => p.ToString("0.00"))));

            sb.Append("]. Biases: [");
            sb.Append(string.Join(", ", Biases.Select(p => p.ToString("0.00"))));

            if (Weights != null)
            {
                sb.Append("]. Weights: [");
                sb.Append(string.Join(", ", Weights.Select(p => p.ToString("0.00"))));

            }

            sb.Append("]");


            Console.WriteLine(sb.ToString());
        }

        public void SetActivation(double[] input)
        {
            Array.Copy(input, Activations, input.Length);
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static double DSigmoid(double x)
        {
            return x * (1 - x);
        }

        public static double DSigmoidS(double x)
        {
            return DSigmoid(Sigmoid(x)); //sigmoid(x) * (1 - sigmoid(x))
        }

        public void ForwardPass(NNLayer nextLayer)
        {
            for (var next = 0; next < nextLayer.Size; next++)
            {
                var acc = nextLayer.Biases[next];
                for (var curr = 0; curr < Size; curr++)
                    acc += Activations[curr] * Weights[next + nextLayer.Size * curr];
                nextLayer.Activations[next] = Sigmoid(acc);
            }
        }

        public double CalculateError(double[] expected)
        {
            double acc = 0;
            for (var i = 0; i < Size; i++)
                acc += .5 * Math.Pow(expected[i] - Activations[i], 2);
            return acc;
        }
    }
}