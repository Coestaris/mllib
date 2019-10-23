using System;
using System.Drawing;
using ml.AI.CNN;
using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        private static Random _random = new Random();

        public static void Main(string[] args)
        {
            var bitmap = new Bitmap("img.png");
            var kernel = new Matrix(3, 3,
                new double[] {0,1,0,0,2,0,0,1,0});

            var layer = new ConvolutionalLayer(
                new Size(bitmap.Width, bitmap.Height),
                kernel);

            var data = new InputData(bitmap);
            data.Pad(kernel);
            data.ToBitmap().Save("padded.png");

            layer.ForwardPass(data.Data);
            //layer.ToBitmap().Save("result.png");
        }
    }
}