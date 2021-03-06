using System;
using System.Collections.Generic;
using System.Drawing;

namespace MLLib.AI.CNN.Layers
{
    public class SubsamplingLayer : CNNLayer
    {
        public int FilterSize;
        public int Stride;
        public int Pad;

        internal int[] _oldX;
        internal int[] _oldY;

        internal SubsamplingLayer() {}

        public SubsamplingLayer(int filterSize, int stride, int pad)
        {
            FilterSize = filterSize;
            Stride = stride;
            Pad = pad;
        }

        public override void BackwardPass()
        {
            InVolume.SetDConstant(0);
            var n = 0;
            for (var d = 0; d < OutDepth; d++)
            {
                var x = -Pad;
                for (var ax = 0; ax < OutSize.Width; ax++, x += Stride)
                {
                    var y = -Pad;
                    for (var ay = 0; ay < OutSize.Height; ay++, y += Stride)
                    {
                        var chain = OutVolume.GetGrad(ax, ay, d);
                        InVolume.AddGrad(_oldX[n], _oldY[n], d, chain);
                        n++;
                    }
                }
            }
        }

        public override void Setup()
        {
            OutDepth = InDepth;
            OutSize = new Size(
                (int) Math.Floor((InSize.Width + Pad * 2 - FilterSize) / (double) Stride + 1),
                (int) Math.Floor((InSize.Height + Pad * 2 - FilterSize) / (double) Stride + 1));

            _oldX = new int[OutSize.Width * OutSize.Height * OutDepth];
            _oldY = new int[OutSize.Width * OutSize.Height * OutDepth];

            base.Setup();
        }

        public override Volume ForwardPass(Volume volume)
        {
            var xPos = 0;
            var yPos = 0;
            var n = 0;

            for (var d = 0; d < OutDepth; d++)
            {
                var x = -Pad;
                for (var ax = 0; ax < OutSize.Width; x += Stride, ax++)
                {
                    var y = -Pad;
                    for (var ay = 0; ay < OutSize.Height; y += Stride, ay++)
                    {
                        var max = double.MinValue;

                        for (var filterX = 0; filterX < FilterSize; filterX++)
                        {
                            var outX = x + filterX;
                            for (var filterY = 0; filterY < FilterSize; filterY++)
                            {
                                var outY = y + filterY;
                                if (outX >= 0 && outX < InSize.Width &&
                                    outY >= 0 && outY < InSize.Height)
                                {
                                    var v = volume.Get(outX, outY, d);
                                    if (v > max)
                                    {
                                        max = v;
                                        xPos = outX;
                                        yPos = outY;
                                    }
                                }
                            }
                        }
                        _oldX[n] = xPos;
                        _oldY[n] = yPos;
                        n++;
                        OutVolume.Set(ax, ay, d, max);
                    }
                }
            }
            return OutVolume;
        }

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = double.NaN;
            L2Decay = double.NaN;
            return new List<Volume>();
        }
    }
}