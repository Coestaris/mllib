using System;

namespace MLLib.AIMath
{
    public static class ActivationFunctions
    {
        public static double TanH(double x)
        {
            var y = Math.Exp(2 * x);
            return (y - 1) / (y + 1);
        }

        public static Matrix Sigmoid(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(Sigmoid);
            return m;
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        public static Matrix SigmoidDS(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(SigmoidDS);
            return m;
        }

        public static double SigmoidDS(double x)
        {
            return x * (1 - x);
        }

        public static Matrix SigmoidD(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(SigmoidD);
            return m;
        }

        public static double SigmoidD(double x)
        {
            return SigmoidDS(Sigmoid(x)); //sigmoid(x) * (1 - sigmoid(x))
        }

        public static Matrix ReLu(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(ReLu);
            return m;
        }

        public static double ReLu(double x)
        {
            return Math.Max(0, x);
        }

        public static Matrix ReLuD(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(ReLuD);
            return m;
        }

        public static double ReLuD(double x)
        {
            if (x >= 0) return 1;
            return 0;
        }

        public static Matrix Softplus(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(Softplus);
            return m;
        }

        public static double Softplus(double x)
        {
            return Math.Log(1 + Math.Exp(x));
        }

        public static Matrix SoftplusD(Matrix m)
        {
            m = new Matrix(m);
            m.ApplyFunction(SoftplusD);
            return m;
        }

        public static double SoftplusD(double x)
        {
            return Sigmoid(x);
        }
    }
}