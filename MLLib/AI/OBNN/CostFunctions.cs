using System;
using ml.AIMath;

namespace ml.AI.OBNN
{
    public abstract class CostFunction
    {
        internal virtual bool ActivationDerivative => false;

        public abstract double GetCost(double[] a, double[] y);
        public abstract double Delta(double z, double a, double y);
    }

    public class QuadraticCostFunction : CostFunction
    {
        internal override bool ActivationDerivative => false;
        public bool UseNorm = false;

        public override double GetCost(double[] a, double[] y)
        {
            if(a.Length != y.Length)
                throw new ArgumentException("Lengths of a and b should be equal");

            //Frobenius norm of vector
            var sum = 0.0;
            if (UseNorm)
            {
                for (var i = 0; i < a.Length; i++)
                    sum += (a[i] - y[i]) * (a[i] - y[i]);
                sum = Math.Sqrt(sum);
            }
            else
            {
                for (var i = 0; i < a.Length; i++)
                    sum += a[i] - y[i];
            }

            return .5 * Math.Pow(sum, 2);
        }

        public override double Delta(double z, double a, double y)
        {
            return (a - y) * ActivationFunctions.SigmoidD(z);
        }
    }

    public class CrossEntropyCostFunction : CostFunction
    {
        internal override bool ActivationDerivative => false;

        public override double GetCost(double[] a, double[] y)
        {
            if(a.Length != y.Length)
                throw new ArgumentException("Lengths of a and b should be equal");

            var sum = 0.0;
            for (var i = 0; i < a.Length; i++)
            {
                var Y = y[i];
                var A = a[i];

                var e = -Y * Math.Log(1e-15 + A) - (1 - Y) * Math.Log(1e-15 + 1 - A);
                if (double.IsNaN(e)) e = 0;

                sum += e;
            }

            return sum;
        }

        public override double Delta(double z, double a, double y)
        {
            return a - y;
        }
    }
}