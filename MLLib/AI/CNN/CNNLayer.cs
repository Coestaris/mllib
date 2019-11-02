using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ml.AI.CNN
{
    public abstract class CNNLayer
    {
        public Volume OutVolume;
        protected internal Volume InVolume;

        public int OutDepth;
        public int InDepth;

        public Size InSize;
        public Size OutSize;

        public CNNLayer PrevLayer;
        public CNNLayer NextLayer;

        public abstract Volume ForwardPass(Volume data);
        public abstract List<Volume> GetLearnableParams(out double L1Decay, out double L2Decay);
        public abstract void BackwardPass();

        public virtual void Setup()
        {
            OutVolume = new Volume(OutSize.Width, OutSize.Height, OutDepth, 0);
        }

        private static float clipValue(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static Color LerpColor(Color a, Color b, float k)
        {
            return Color.FromArgb(
                0,
                (int) clipValue(a.R * k + b.R * (1 - k), 0, 255),
                (int) clipValue(a.G * k + b.G * (1 - k), 0, 255),
                (int) clipValue(a.B * k + b.B * (1 - k), 0, 255));
        }

        public Bitmap ToBitmap(int depth)
        {
            return ToBitmap(depth, Color.Black, Color.White);
        }

        public virtual Bitmap ToBitmap(int depth, Color a, Color b)
        {
            var bmp = new Bitmap(OutSize.Width, OutSize.Height, PixelFormat.Format32bppArgb);

            double min = double.MaxValue, max = double.MinValue;
            for (var d = 0; d < OutDepth; d++)
            {
                for (var x = 0; x < OutSize.Width; x++)
                for (var y = 0; y < OutSize.Height; y++)
                {
                    var v = OutVolume.Get(x, y, d);
                    if (v > max) max = v;
                    if (v < min) min = v;
                }
            }

            for (var x = 0; x < OutSize.Width; x++)
            for (var y = 0; y < OutSize.Height; y++)
            {
                var v = OutVolume.Get(x, y, depth);
                var delta = max - min;
                if (Math.Abs(delta) <= 1) v -= min;
                else v = (v - min) / delta;

                bmp.SetPixel(x, y, LerpColor(a, b, (float)v));
            }

            return bmp;
        }
    }
}