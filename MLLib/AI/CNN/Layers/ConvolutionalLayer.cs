using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ml.AIMath;
using Size = ml.AIMath.Size;

namespace ml.AI.CNN
{
    public class ConvolutionalLayer : CNNLayer
    {
        public readonly Size LayerSize;
        public Size KernelSize => Kernel.Size;

        public readonly double[,] Data;
        public readonly Matrix Kernel;

        public void PadData()
        {

        }

        public void ForwardPass(double[,] data)
        {

        }

        public ConvolutionalLayer(Bitmap bitmap, Matrix kernel)
        {

        }

        public Bitmap ToBitmap()
        {
            var bmp = new Bitmap(LayerSize.Width, LayerSize.Height, PixelFormat.Format8bppIndexed);

            var palette = bmp.Palette;
            var entries = palette.Entries;
            for (var i = 0; i < 256; i++)
            {
                var b = Color.FromArgb((byte)i, (byte)i, (byte)i);
                entries[i] = b;
            }
            bmp.Palette = palette;


            var data = bmp.LockBits(new Rectangle(
                    Point.Empty,
                    new System.Drawing.Size(LayerSize.Width, LayerSize.Height)),
                ImageLockMode.WriteOnly,
                bmp.PixelFormat);

            var pixelData = new byte[LayerSize.Height * LayerSize.Width];
            Data.CopyTo(pixelData, 0);

            var targetStride = data.Stride;
            var scan0 = data.Scan0.ToInt64();
            for (var y = 0; y < LayerSize.Height; y++)
                Marshal.Copy(pixelData, y * LayerSize.Width, new IntPtr(scan0 + y * targetStride),  LayerSize.Width);

            bmp.UnlockBits(data);
            return bmp;
        }
    }
}