using System;
using System.Drawing;

namespace ml.AI.CNN
{
    public abstract class CNNLayer
    {
        protected internal Volume OutVolume;
        protected internal Volume InVolume;

        public int OutDepth;
        public int InDepth;

        public Size InSize;
        public Size OutSize;

        public CNNLayer PrevLayer;
        public CNNLayer NextLayer;

        public abstract Volume ForwardPass(Volume data);

        public virtual void Setup()
        {
            OutVolume = new Volume(OutSize.Width, OutSize.Height, OutDepth, 0);
        }

        public virtual Bitmap ToBitmap(int depth)
        {
            var bmp = new Bitmap(OutSize.Width, OutSize.Height);

            double min = double.MaxValue, max = double.MinValue;
            for (var x = 0; x < OutSize.Width; x++)
            for (var y = 0; y < OutSize.Height; y++)
            {
                var v = OutVolume.Get(x, y, depth);
                if (v > max) max = v;
                if (v < min) min = v;
            }


            for (var x = 0; x < OutSize.Width; x++)
            for (var y = 0; y < OutSize.Height; y++)
            {
                var v = OutVolume.Get(x, y, depth);
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