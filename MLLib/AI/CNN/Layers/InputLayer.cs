using System;
using System.Drawing;
using ml.AIMath;

namespace ml.AI.CNN.Layers
{
    public class InputLayer : CNNLayer
    {
        public Size Size;
        public Size PaddedLayerSize;
        public Size PadSize;

        public double[,] Data;

        public static Size PadSizeFromKernelSize(Matrix matrix)
        {
            return new Size(
                (int)Math.Floor(matrix.Size.Width / 2.0),
                (int)Math.Floor(matrix.Size.Height / 2.0));
        }

        public static Volume BitmapToVolume(Bitmap bitmap, bool grayscale)
        {
            Volume volume;
            if (grayscale)
            {
                volume = new Volume(bitmap.Width, bitmap.Height, 1, 0);
                for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                    volume.Set(x, y, 0, NormalizeColor(bitmap.GetPixel(x, y)));
            }
            else
            {
                volume = new Volume(bitmap.Width, bitmap.Height, 3, 0);
                for(var d = 0; d < 3; d++)
                for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var c = bitmap.GetPixel(x, y);
                    var value = 0;
                    if (d == 0) value = c.R;
                    else if (d == 1) value = c.G;
                    else value = c.B;
                    volume.Set(x, y, d, value);
                }
            }

            return volume;
        }

        private void Pad(int xpad, int ypad)
        {
            PaddedLayerSize = new Size(
                Size.Width + xpad * 2,
                Size.Height + ypad * 2);

            Data = new double[PaddedLayerSize.Width, PaddedLayerSize.Height];
        }

        public static double NormalizeColor(Color color)
        {
            return (color.R + color.B + color.G) / (3.0 * 256);
        }

        public InputLayer(Size size, Size padSize)
        {
            Size = new Size(size.Width, size.Height);

            Data = new double[Size .Width, Size .Height];
            Pad(padSize.Width, padSize.Height);
            PadSize = padSize;
        }

        public Bitmap ToBitmap()
        {
            var size = Size;
            if (!PaddedLayerSize.IsEmpty)
                size = PaddedLayerSize;

            var bmp = new Bitmap(size.Width, size.Height);
            for(var x = 0; x < bmp.Width; x++)
            for(var y = 0; y < bmp.Height; y++)
                bmp.SetPixel(x ,y, Color.FromArgb(
                    (int)(Data[x, y] * 256),
                    (int)(Data[x, y] * 256),
                    (int)(Data[x, y] * 256)));

            return bmp;
        }

        public override Volume ForwardPass(Volume volume) { return new Volume(); }
    }
}