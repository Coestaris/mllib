namespace MLLib.AI.OBNN
{
    internal struct DerivativePack
    {
        public double   Delta;
        public double[] dW;
        public double   dB;

        public DerivativePack(double delta, double[] dW, double dB)
        {
            this.Delta = delta;
            this.dW = dW;
            this.dB = dB;
        }
    }
}