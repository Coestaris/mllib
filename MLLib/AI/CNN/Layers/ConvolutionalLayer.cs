using System;
using System.Drawing;
using ml.AIMath;

namespace ml.AI.CNN.Layers
{
    public class ConvolutionalLayer : CNNLayer
    {
        private static GaussianRandom _random = new GaussianRandom();

        public int FilterSize;
        public int FiltersCount;
        public int Pad;
        public int Stride;

        public double[] Biases;
        public Volume[] Kernels;

        public override Volume ForwardPass(Volume volume)
        {
            for (var d = 0; d < OutDepth; d++)
            {
                var kernel = Kernels[d];
                var x = -Pad;
                for (var ax = 0; ax < OutSize.Width; x += Stride, ax += Stride)
                {
                    var y = -Pad;
                    for (var ay = 0; ay < OutSize.Height; y += Stride, ay += Stride)
                    {
                        var sum = 0.0;

                        for (var filterX = 0; filterX < FilterSize; filterX++)
                        {
                            var outX = x + filterX;
                            for (var filterY = 0; filterY < FilterSize; filterY++)
                            {
                                var outY = y + filterY;
                                if (outX >= 0 && outX < OutSize.Width &&
                                    outY >= 0 && outY < OutSize.Height)
                                {
                                    for (var fd = 0; fd < kernel.Depth; fd++)
                                    {
                                        sum += kernel.Get(filterX, filterY, fd) *
                                               volume.Get(outX, outY, fd);
                                    }
                                }
                            }
                        }

                        sum += Biases[d];
                        _returnVolume.Set(ax, ay, d, sum);
                    }
                }
            }
            return _returnVolume;
        }

        public ConvolutionalLayer(
            int filtersCount,
            int filterSize,
            int pad,
            int stride)
        {
            Kernels = new Volume[filtersCount];

            Biases = new double[filtersCount];
            //_random.Fill(Biases);

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

            Kernels[0].Weights = new double[] { 1, 0, -1, 2, 0, -2, -1, 0, -1 };
            Kernels[1].Weights = new double[] { 1, 2, 1, 0, 0, 0, -1, -2, 1, };

            Console.WriteLine("1");
            Kernels[0].Print(0);

            Console.WriteLine("2");
            Kernels[1].Print(0);

            _returnVolume = new Volume(OutSize.Width, OutSize.Height, OutDepth, 0);
        }
    }
}