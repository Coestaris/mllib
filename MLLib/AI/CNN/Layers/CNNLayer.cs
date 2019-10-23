namespace ml.AI.CNN.Layers
{
    public abstract class CNNLayer
    {
        public CNNLayer PrevLayer;
        public CNNLayer NextLayer;

        public abstract void ForwardPass(double[,] data);
    }
}