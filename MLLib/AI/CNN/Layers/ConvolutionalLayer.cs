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
        public Size LayerSize;
        public Size Stride;
        public Size KernelSize;

        public double[] Biases;
        public List<Matrix> Kernels;
        public Size PrevPad;

        public override Volume ForwardPass(Volume volume)
        {
            for (var d = 0; d < Depth; d++)
            {
                for (var x = 0; x < LayerSize.Width;  x += Stride.Width)
                for (var y = 0; y < LayerSize.Height; y += Stride.Height)
                {
                    double sum = 0;
                    for (var m = 0; m < KernelSize.Width; m++)
                    for (var n = 0; n < KernelSize.Height; n++)
                        sum += Kernels[d].Data[m, n] *
                               data[x + m, y + n];

                    Data[d][x, y] = ActivationFunctions.Sigmoid(sum + Biases[d]);
                }
            }
        }

        public ConvolutionalLayer(
            List<Matrix> kernels,
            Size prevLayerSize,
            Size prevLayerPad,
            Size stride)
        {
            KernelSize = kernels[0].Size;
            LayerSize = new Size(
                    (int)Math.Floor((prevLayerSize.Width + 2 * prevLayerPad.Width - KernelSize.Width) / (double)stride.Width + 1),
                    (int)Math.Floor((prevLayerSize.Height + 2 * prevLayerPad.Height - KernelSize.Height) / (double)stride.Height + 1));
            Kernels = kernels;
            Stride = stride;
            Depth = kernels.Count;
            PrevPad = prevLayerPad;

            Biases = new double[Depth];
            Data = new double[Depth][,];
            for(var i = 0; i < Depth; i++)
                Data[i] = new double[LayerSize.Width, LayerSize.Height];
        }

        public Bitmap ToBitmap(int index)
        {

            var bmp = new Bitmap(LayerSize.Width, LayerSize.Height);
            for(var x = 0; x < bmp.Width; x++)
            for(var y = 0; y < bmp.Height; y++)
                bmp.SetPixel(x ,y, Color.FromArgb(
                        (int)(Data[index][x, y] * 256),
                        (int)(Data[index][x, y] * 256),
                        (int)(Data[index][x, y] * 256)));

            return bmp;
        }
    }
}