using System;
using System.Drawing;
using ml.AIMath;

namespace ml.AI.CNN.Layers
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
    }
}