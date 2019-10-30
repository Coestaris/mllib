using System.Collections.Generic;
using System.Drawing;
using ml.AI.CNN.Layers;

namespace ml.AI.CNN.Layers
{
    public class FullyConnectedLayer : CNNLayer
    {
        public int NeuronsCount;
        public double L1Decay;
        public double L2Decay;

        public Volume[] Weights;
        public Volume   Biases;

        internal int _inputsCount;

        internal FullyConnectedLayer() {}

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = this.L1Decay;
            L2Decay = this.L2Decay;

            var list = new List<Volume>();
            list.AddRange(Weights);
            list.Add(Biases);
            return list;
        }

        public FullyConnectedLayer(int neuronsCount, int l1decay = 0, int l2decay = 1)
        {
            NeuronsCount = neuronsCount;
            L1Decay = l1decay;
            L2Decay = l2decay;
        }

        public override void Setup()
        {
            OutDepth = NeuronsCount;
            OutSize = new Size(1, 1);
            Biases = new Volume(1, 1, NeuronsCount, .1);
            Weights = new Volume[NeuronsCount];

            _inputsCount = InSize.Width * InSize.Height * InDepth;
            for(int i = 0; i < NeuronsCount; i++)
                Weights[i] = new Volume(1, 1, _inputsCount);

            base.Setup();
        }

        public override void BackwardPass()
        {
            InVolume.SetDConstant(0);
            for (var i = 0; i < OutDepth; i++)
            {
                var filter = Weights[i];
                var chain = OutVolume.dWeights[i];

                for (var d = 0; d < _inputsCount; d++)
                {
                    InVolume.dWeights[d] += filter.Weights[d] * chain;
                    filter.dWeights[d] += InVolume.Weights[d] * chain;
                }
                Biases.dWeights[i] += chain;
            }

        }

        public override Volume ForwardPass(Volume volume)
        {
            var rawInput = volume.WeightsRaw;
            var rawOutput = OutVolume.WeightsRaw;
            var rawBiases = Biases.WeightsRaw;
            for (var i = 0; i < NeuronsCount; i++)
            {
                var sum = 0.0;
                var rawNeuron = Weights[i].WeightsRaw;
                for (var d = 0; d < _inputsCount; d++)
                    sum += rawInput[d] * rawNeuron[d];
                sum += rawBiases[i];
                rawOutput[i] = sum;
            }

            return OutVolume;
        }
    }
}