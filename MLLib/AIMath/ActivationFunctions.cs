using System;

namespace ml.AIMath
{
    public static class ActivationFunctions
    {
        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static double DSigmoidS(double x)
        {
            return x * (1 - x);
        }

        public static double DSigmoid(double x)
        {
            return DSigmoidS(Sigmoid(x)); //sigmoid(x) * (1 - sigmoid(x))
        }

        public static double ReLu(double x)
        {
            return Math.Max(0, x);
        }

        public static double dReLu(double x)
        {
            if (x >= 0) return 1;
            return 0;
        }

        public static double Softplus(double x)
        {
            return Math.Log(1 + Math.Exp(x));
        }

        public static double dSoftplus(double x)
        {
            return Sigmoid(x);
        }
    }
}