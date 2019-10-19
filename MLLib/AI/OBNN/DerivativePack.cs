namespace ml.AI.OBNN
{
    struct DerivativePack
    {
        public double   dA;
        public double   dZ;
        public double[] dW;
        public double   dB;

        public DerivativePack(double dA, double dZ, double[] dW, double dB)
        {
            this.dA = dA;
            this.dZ = dZ;
            this.dW = dW;
            this.dB = dB;
        }
    }
}