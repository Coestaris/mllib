using System;

namespace ml.AI.CNN.Layers
{
    public class SoftmaxLayer : CNNLayer
    {
        public override Volume ForwardPass(Volume volume)
        {
            var max = double.MinValue;
            var rawInput = volume.WeightsRaw;
            var rawOutput = OutVolume.WeightsRaw;

            for(var i = 0; i < rawInput.Length; i++)
                if (rawInput[i] > max) max = rawInput[i];

            var esum = 0d;
            for (var i = 0; i < rawInput.Length; i++)
            {
                var e = Math.Exp(rawInput[i] - max);
                rawOutput[i] = e;
                esum += e;
            }

            for (var i = 0; i < rawInput.Length; i++)
                rawOutput[i] /= esum;

            return OutVolume;
        }

        public override void Setup()
        {
            OutDepth = InDepth;
            OutSize = InSize;

            if(InSize.Width != 1 || InSize.Height != 1)
                throw new ArgumentException("You can use softmax only for vectors");

            base.Setup();
        }
    }
}