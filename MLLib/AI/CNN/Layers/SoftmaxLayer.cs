using System;

namespace ml.AI.CNN.Layers
{
    public class SoftmaxLayer : OutputLayer
    {
        internal double[] _es;

        public override void BackwardPass() { }
        public override double BackwardPassLoss(int correctIndex)
        {
            var rawIn = InVolume.dWeightsRaw;
            for (var i = 0; i < OutDepth; i++)
            {
                var indicator = i == correctIndex ? 1.0 : 0.0;
                var mul = -(indicator - _es[i]);
                rawIn[i] = mul;
            }

            return -Math.Log(_es[correctIndex]);
        }

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
                _es[i] = (rawOutput[i] /= esum);

            return OutVolume;
        }

        public override void Setup()
        {
            OutDepth = InDepth;
            OutSize = InSize;
            _es = new double[OutDepth];

            if(InSize.Width != 1 || InSize.Height != 1)
                throw new ArgumentException("You can use softmax only for vectors");

            base.Setup();
        }
    }
}