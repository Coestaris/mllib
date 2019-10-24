using System.Drawing;

namespace ml.AI.CNN.Layers
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
            for(var x = 0; x < bmp.Width; x++)
            for(var y = 0; y < bmp.Height; y++)
                bmp.SetPixel(x ,y, Color.FromArgb(
                    (int)(_returnVolume.Get(x, y, depth) * 256),
                    (int)(_returnVolume.Get(x, y, depth)* 256),
                    (int)(_returnVolume.Get(x, y, depth) * 256)));

            return bmp;
        }
    }
}