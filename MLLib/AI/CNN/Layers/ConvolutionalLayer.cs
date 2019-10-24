using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ml.AI.CNN.Layers;
using ml.AIMath;

namespace ml.AI.CNN
{
    public class ConvolutionalLayer : CNNLayer
    {
        public int FilterSize;
        public Size Pad;
        public Size Stride;

        public double[] Biases;
        public Matrix[] Kernels;

        public override Volume ForwardPass(Volume volume)
        {
            for (var d = 0; d < OutDepth; d++)
            {
                var kernel = Kernels[d];
            }

            return _returnVolume = volume;
        }

        public ConvolutionalLayer(
            int filtersCount,
            int filterSize,
            Size pad,
            Size stride)
        {
            Kernels = new Matrix[filtersCount];
            for(int i = 0; i < filtersCount; i++)
                Kernels[i] = new Matrix(FilterSize, FilterSize);

            Biases = new double[filtersCount];
            OutDepth = filtersCount;

            FilterSize = filterSize;
            Pad = pad;
            Stride = stride;
        }

        public override void Setup()
        {
            OutSize = new Size(
                (int) Math.Floor((InSize.Width + Pad.Width * 2 - FilterSize) / (double) Stride.Width + 1),
                (int) Math.Floor((InSize.Height + Pad.Height * 2 - FilterSize) / (double) Stride.Height + 1));
        }
    }
}