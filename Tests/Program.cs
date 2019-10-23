using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ml.AI.CNN;
using ml.AI.CNN.Layers;
using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            var bitmap = new Bitmap("img.png");
            var kernel1 = new Matrix(3, 3, new double[] {1,2,1,0,0,0,-1,-2,-1});
            var kernel2 = new Matrix(3, 3, new double[] {1,0,-1,2,0,-2,1,0,-1});

            var network = new ConvolutionalNeuralNetwork();

            var layer1 = new InputLayer(bitmap.Size, InputLayer.PadSizeFromKernelSize(kernel1));
            var layer2 = new ConvolutionalLayer(
                new List<Matrix>() {kernel1, kernel2},
                layer1.Size,
                layer1.PadSize,
                new Size(1, 1));

            network.PushLayer(layer1);
            network.PushLayer(layer2);

            network.ForwardPass(bitmap);

            layer1.ToBitmap().Save("0_padded.png");
            for(var i = 0; i < layer2.Depth; i++)
                layer2.ToBitmap(i).Save($"1_conv{i}.png");
        }
    }
}