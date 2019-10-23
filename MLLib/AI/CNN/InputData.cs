using System.Drawing;
using ml.AIMath;

namespace ml.AI.CNN
{
    public class InputData
    {
        public Size Size;
        public Size PaddedSize;
        public int XPad;
        public int YPad;

        public double[,] Data;

        public void Pad(Matrix kernel)
        {
            Pad(kernel.Rows / 2);
        }

        public void Pad(int pad)
        {
            Pad(pad, pad);
        }

        public void Pad(int xpad, int ypad)
        {
            XPad = xpad;
            YPad = ypad;

            PaddedSize = new Size(
                Size.Width + xpad * 2,
                Size.Height + ypad * 2);

            var newData = new double[PaddedSize.Width, PaddedSize.Height];
            for (int x = xpad; x < PaddedSize.Width - xpad; x++)
            for (int y = ypad; y < PaddedSize.Height - ypad; y++)
            {
                newData[x, y] = Data[x - xpad, y - ypad];
            }

            Data = newData;
        }

        public static double NormalizeColor(Color color)
        {
            return (color.R + color.B + color.G) / (3.0 * 256);
        }

        public InputData(Bitmap bitmap)
        {
            Size = new Size(bitmap.Width, bitmap.Height);

            Data = new double[Size .Width, Size .Height];
            for (var x = 0; x < Size .Width; x++)
            for (var y = 0; y < Size .Height; y++)
                Data[x, y] = NormalizeColor(bitmap.GetPixel(x, y));
        }

        public Bitmap ToBitmap()
        {
            var size = Size;
            if (PaddedSize.IsEmpty)
                size = PaddedSize;

            var bmp = new Bitmap(size.Width, size.Height);
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