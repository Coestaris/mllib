using System.Collections.Generic;

namespace ml.AI.CNN
{
    public class ConvolutionalNeuralNetwork
    {
        public readonly List<ConvolutionalLayer> Layers;

        public ConvolutionalNeuralNetwork()
        {
            Layers = new List<ConvolutionalLayer>();
        }
    }
}