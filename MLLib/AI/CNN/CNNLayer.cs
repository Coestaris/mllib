using System;
using System.Drawing;

namespace ml.AI.CNN
{
    public abstract class CNNLayer
    {
        protected Volume _returnVolume;

        public int OutDepth;
        public int InDepth;

        public Size InSize;
        public Size OutSize;

        public CNNLayer PrevLayer;
        public CNNLayer NextLayer;

        public abstract Volume ForwardPass(Volume data);
        public abstract void Setup();

        public virtual Bitmap ToBitmap(int depth)
        {
            var bmp = new Bitmap(OutSize.Width, OutSize.Height);

            double min = double.MaxValue, max = double.MinValue;
            for (var x = 0; x < OutSize.Width; x++)
            for (var y = 0; y < OutSize.Height; y++)
            {
                var v = _returnVolume.Get(x, y, depth);
                if (v > max) max = v;
                if (v < min) min = v;
            }


            for (var x = 0; x < OutSize.Width; x++)
            for (var y = 0; y < OutSize.Height; y++)
            {
                var v = _returnVolume.Get(x, y, depth);
                var delta = max - min;
                if (Math.Abs(delta) <= 1) v -= min;
                else v = (v - min) / delta;

                bmp.SetPixel(x, y, Color.FromArgb(
                    (int)(v * 255),
                    (int)(v * 255),
                    (int)(v * 255)));
            }

            return bmp;
        }
    }
}