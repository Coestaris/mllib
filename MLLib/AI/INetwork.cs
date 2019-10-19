namespace ml.AI
{
    public interface INetwork
    {
        double[] ForwardPass(double[] input);
        void BackProp(double[] expected);
        void ApplyNudge(int count);
        double CalculateError(double[] expected);
    }
}