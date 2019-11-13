using System;
using MLLib.AIMath;

namespace MLLib.AI
{
    public abstract class Network
    {
        public abstract double[] ForwardPass(double[] input);
        public abstract void BackProp(double[] expected);
        public abstract void ApplyNudge(int count, int totalCount);
        public abstract double CalculateError(double[] expected);
        public abstract void Print();
        public abstract void FillRandom(Random random = null);
        public abstract void FillGaussianRandom(GaussianRandom gaussianRandom = null);
    }
}