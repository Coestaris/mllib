namespace ml.AI.CNN.Layers
{
    public class NonLinearLayer : CNNLayer
    {
        public override Volume ForwardPass(Volume volume) { return new Volume(); }
        public override void Setup() {}
    }
}