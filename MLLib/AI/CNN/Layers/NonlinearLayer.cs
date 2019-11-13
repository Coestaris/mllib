using System;
using System.Collections.Generic;
using MLLib.AIMath;

namespace MLLib.AI.CNN.Layers
{
    public class ReLuLayer : CNNLayer
    {
        public override Volume ForwardPass(Volume volume)
        {
            var raw = volume.WeightsRaw;
            var destRaw = OutVolume.WeightsRaw;

            for (var i = 0; i < raw.Length; i++)
                destRaw[i] = raw[i] > 0 ? raw[i] : 0;

            return OutVolume;
        }

        public override void Setup()
        {
            OutDepth = InDepth;
            OutSize = InSize;

            base.Setup();
        }

        public override void BackwardPass()
        {
            InVolume.SetDConstant(0);
            var rawOut = OutVolume.WeightsRaw;
            var rawDOut = OutVolume.dWeightsRaw;
            var rawDIn = InVolume.WeightsRaw;

            for (var i = 0; i < InVolume.dWeightsRawLen; i++)
            {
                if (rawOut[i] <= 0) rawDIn[i] = 0;
                else rawDIn[i] = rawDOut[i];
            }
        }

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = double.NaN;
            L2Decay = double.NaN;
            return new List<Volume>();
        }
    }

    public class SigmoidLayer : CNNLayer
    {
        public override Volume ForwardPass(Volume volume)
        {
            var raw = volume.WeightsRaw;
            var destRaw = OutVolume.WeightsRaw;

            for (var i = 0; i < raw.Length; i++)
                destRaw[i] = ActivationFunctions.Sigmoid(raw[i]);

            return OutVolume;
        }

        public override void Setup()
        {
            OutDepth = InDepth;
            OutSize = InSize;

            base.Setup();
        }

        public override void BackwardPass() { }

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = double.NaN;
            L2Decay = double.NaN;
            return new List<Volume>();
        }
    }
}