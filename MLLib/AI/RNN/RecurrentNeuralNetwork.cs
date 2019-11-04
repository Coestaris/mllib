using System.Linq;
using ml.AIMath;

namespace ml.AI.RNN
{
    //Single layer char based RNN
    public class RecurrentNeuralNetwork
    {
        public int HiddenCount;
        public int OutputCount;
        public double LR;

        //x - input
        //h - hidden layer
        //y - output
        private Matrix hBias;
        private Matrix yBias;
        private Matrix xhW;
        private Matrix hhW;
        private Matrix hyW;

        public char[] Data;
        public char[] Vocab;

        public RecurrentNeuralNetwork(string input, int hiddenCount, int outputCount, double lr)
        {
            Data = input.ToCharArray();
            Vocab = Data.Distinct().ToArray();

            HiddenCount = hiddenCount;
            OutputCount = outputCount;
            LR = lr;
            xhW = new Matrix(HiddenCount, Vocab.Length).Multiply();

        }

        public void ForwardPass()
        {

        }

        public void BackwardPass()
        {

        }

        public void Train(char[] chars)
        {

        }
    }
}