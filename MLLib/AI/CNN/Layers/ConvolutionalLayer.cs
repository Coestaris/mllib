using System;
using System.Collections.Generic;
using System.Drawing;
using ml.AIMath;

namespace ml.AI.CNN.Layers
{
    public class ConvolutionalLayer : CNNLayer
    {
        private static GaussianRandom _random = new GaussianRandom();

        public double L1Decay;
        public double L2Decay;

        public int FilterSize;
        public int FiltersCount;
        public int Pad;
        public int Stride;

        public Volume   Biases;
        public Volume[] Kernels;

        public override void BackwardPass()
        {
            for (var d = 0; d < OutDepth; d++)
            {
                var filter = Kernels[d];
                var y = -Pad;
                for (var ay = 0; ay < OutSize.Height; y += Stride, ay++)
                {
                    var x = -Pad;
                    for (var ax = 0; ax < OutSize.Width; x += Stride, ax++)
                    {
                        var chain = OutVolume.GetGrad(ax, ay, d);
                        for (var fy = 0; fy < FilterSize; fy++)
                        {
                            var oy = y + fy;
                            for (var fx = 0; fx < FilterSize; fx++)
                            {
                                var ox = x + fx;
                                 if (oy < 0 || oy >= OutSize.Height || ox < 0 || ox >= OutSize.Width) continue;

                                for (var fd = 0; fd < filter.Depth; fd++)
                                {
                                    var ix1 = ((OutSize.Width * oy) + ox) * InVolume.Depth + fd;
                                    var ix2 = ((filter.SX * fy) + fx) * filter.Depth + fd;
                                    filter.dWeights[ix2] += InVolume.Weights[ix1] * chain;
                                    InVolume.dWeights[ix1] += filter.Weights[ix2] * chain;
                                }
                            }
                        }
                        Biases.dWeights[d] += chain;
                    }
                }
            }
        }

        public override Volume ForwardPass(Volume volume)
        {
            var rawBiases = Biases.WeightsRaw;
            for (var d = 0; d < OutDepth; d++)
            {
                var kernel = Kernels[d];
                var x = -Pad;
                for (var ax = 0; ax < OutSize.Width; x += Stride, ax++)
                {
                    var y = -Pad;
                    for (var ay = 0; ay < OutSize.Height; y += Stride, ay++)
                    {
                        var sum = 0.0;

                        for (var filterX = 0; filterX < FilterSize; filterX++)
                        {
                            var outX = x + filterX;
                            for (var filterY = 0; filterY < FilterSize; filterY++)
                            {
                                var outY = y + filterY;
                                if (outX >= 0 && outX < InSize.Width &&
                                    outY >= 0 && outY < InSize.Height)
                                {
                                    for (var fd = 0; fd < kernel.Depth; fd++)
                                    {
                                        sum += kernel.Get(filterX, filterY, fd) *
                                               volume.Get(outX, outY, fd);
                                    }
                                }
                            }
                        }

                        sum += rawBiases[d];
                        OutVolume.Set(ax, ay, d, sum);
                    }
                }
            }
            return OutVolume;
        }

        internal ConvolutionalLayer() {}

        public override List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay)
        {
            L1Decay = this.L1Decay;
            L2Decay = this.L2Decay;

            var list = new List<Volume>();
            list.AddRange(Kernels);
            list.Add(Biases);
            return list;
        }

        public ConvolutionalLayer(
            int filtersCount,
            int filterSize,
            int pad,
            int stride)
        {
            Kernels = new Volume[filtersCount];

            Biases = new Volume(1, 1, filtersCount);
            OutDepth = filtersCount;

            FilterSize = filterSize;
            Pad = pad;
            Stride = stride;
        }

        public override void Setup()
        {
            OutSize = new Size(
                (int) Math.Floor((InSize.Width + Pad * 2 - FilterSize) / (double) Stride + 1),
                (int) Math.Floor((InSize.Height + Pad * 2 - FilterSize) / (double) Stride + 1));

            for (int i = 0; i < Kernels.Length; i++)
            {
                Kernels[i] = new Volume(FilterSize, FilterSize, InDepth);
            }

/*            Console.WriteLine("1");
            Kernels[0].Print(0);

            Console.WriteLine("2");
            Kernels[1].Print(0);*/

            base.Setup();
        }
    }
}