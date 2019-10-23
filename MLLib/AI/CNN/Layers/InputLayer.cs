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

        public override void ForwardPass(double[,] data)
        {
            throw new NotSupportedException();
        }

        public void ForwardPass(Bitmap bitmap)
        {
            for (var x = 0; x < Size.Width; x++)
            for (var y = 0; y < Size.Height; y++)
                Data[x + PadSize.Width, y + PadSize.Height] = NormalizeColor(bitmap.GetPixel(x, y));

            NextLayer.ForwardPass(Data);
        }
    }
}