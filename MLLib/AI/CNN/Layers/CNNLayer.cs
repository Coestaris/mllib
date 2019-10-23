using System.Drawing;

namespace ml.AI.CNN.Layers
{
    public abstract class CNNLayer
    {
        public int OutDepth;
        public int InDepth;

        public Size InSize;
        public Size OutSize;

        public CNNLayer PrevLayer;
        public CNNLayer NextLayer;

        public abstract Volume ForwardPass(Volume data);
    }
}