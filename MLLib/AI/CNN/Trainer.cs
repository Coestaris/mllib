namespace ml.AI.CNN
{
    public class Trainer
    {
        private ConvolutionalNeuralNetwork _network;
        public double LearningRate;

        public Trainer(ConvolutionalNeuralNetwork network, double learningRate)
        {
            LearningRate = learningRate;
            _network = network;
        }

        public double Train(Volume input, int output)
        {
            _network.ForwardPass(input);
            var loss = _network.BackwardPass(output);

            foreach (var layer in _network.Layers)
            {
                double l1, l2;
                var p = layer.GetLearnableParams(out l1, out l2);

                foreach (var volume in p)
                {
                    var rawParams = volume.WeightsRaw;
                    var rawGrad = volume.dWeightsRaw;

                    for (var j = 0; j < rawParams.Length; j++)
                    {
                        var l1Grad = l1 * (rawParams[j] > 0 ? 1 : -1);
                        var l2Grad = l2 * (rawParams[j]);

                        var grad = (l2Grad + l1Grad + rawGrad[j]) / (1); //batch size = 1

                        rawParams[j] += -LearningRate * grad;
                    }
                }
            }

            return loss;
        }
    }
}