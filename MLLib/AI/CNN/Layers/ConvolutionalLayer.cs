using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ml.AIMath;

namespace ml.AI.CNN
{
    public class ConvolutionalLayer : CNNLayer
    {
        public readonly Size LayerSize;
        public Size KernelSize => Kernel.Size;

        public double[,] Data;
        public readonly Matrix Kernel;

        public void PadData()
        {

        }

        public void ForwardPass(double[,] data)
        {
        }

        public ConvolutionalLayer(Size size, Matrix kernel)
        {
            LayerSize = size;
            LayerSize.Width  += (int)(Math.Ceiling(kernel.Size.Width  / 2.0) * 2);
            LayerSize.Height += (int)(Math.Ceiling(kernel.Size.Height / 2.0) * 2);

            Kernel = kernel;
            Data = new double[size.Width, size.Height];
        }

        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(LayerSize.Width, LayerSize.Height);
            for(var x = 0; x < bmp.Width; x++)
            for(var y = 0; y < bmp.Height; y++)
                bmp.SetPixel(x ,y, Color.FromArgb(
                        (int)(Data[x, y] * 256),
                        (int)(Data[x, y] * 256),
                        (int)(Data[x, y] * 256)));

            return bmp;
        }
    }
}