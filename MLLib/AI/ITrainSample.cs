namespace ml.AI
{
    public interface ITrainSample
    {
        double[] ToTrainData();
        double[] ToExpected();
    }
}