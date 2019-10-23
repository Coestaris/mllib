using System.Drawing;

namespace ml.AI.CNN
{
    public class InputData
    {
        public Size Size;
        public double[,] Data;

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
    }
}