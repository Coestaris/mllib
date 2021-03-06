using System;
using System.Collections.Generic;
using System.Drawing;
using MLLib.AIMath;

namespace MLLib.AI.CNN.Layers
{
    public class InputLayer : CNNLayer
    {
        internal InputLayer() {}

        public InputLayer(Size size, bool rgb = false)
        {
            OutSize = size;
            if (rgb) OutDepth = 3;
            else OutDepth = 1;
        }

        public override Volume ForwardPass(Volume volume) { return OutVolume = volume; }

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = double.NaN;
            L2Decay = double.NaN;
            return new List<Volume>();
        }

        public override void BackwardPass() { }
    }
}