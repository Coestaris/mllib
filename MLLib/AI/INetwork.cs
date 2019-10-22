using System;
using ml.AIMath;

namespace ml.AI
{
    public interface INetwork
    {
        double[] ForwardPass(double[] input);
        void BackProp(double[] expected);
        void ApplyNudge(int count, int totalCount);
        double CalculateError(double[] expected);

        void Print();
        void FillRandom(Random random = null);
        void FillGaussianRandom(GaussianRandom gaussianRandom = null);
    }
}