namespace MLLib.AI.CNN
{
    public abstract class OutputLayer : CNNLayer
    {
        public abstract double BackwardPassLoss(int correctIndex);
    }
}