namespace MLLib.AI
{
    public abstract class TrainSample
    {
        public abstract double[] ToTrainData();
        public abstract double[] ToExpected();
        public abstract bool CheckAssumption(double[] output);
    }
}