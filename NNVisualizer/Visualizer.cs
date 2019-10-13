using ml.AI;
using OpenTK;

namespace NNVisualizer
{
    public class NNVisualizer : WindowHandler
    {
        public NeuralNetwork Network;

        public NNVisualizer(Window window, NeuralNetwork network) : base(window)
        {
            Network = network;
            Window.UpdateFunc = Update;
        }

        public void Update()
        {

        }

        public override void Start()
        {
            /*
            foreach (var layer in Network.Layers)
            {
                /*AddObject(new Circle(
                    new Vector2(200, 200), new Vector3(1, 1, 1), 20));#1#
            }
*/

            base.Start();
        }
    }
}