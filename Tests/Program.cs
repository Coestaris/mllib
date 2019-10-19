using ml.AI.OBNN;
using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*var m1 = new Matrix(2, 3, 1,2,3,4,5,6);
            var m2 = new Matrix(3, 3, 3,6,9,4,7,10,5,8,11);

            m1.Print();
            m2.Print();

            (m1 * m2).Print();*/

            var network = new MBNeuralNetwork(new[] {2, 3, 2});
            network.FillGaussianRandom();
            network.Print();

            network.ForwardPass(new double[] {2, 4});
        }
    }
}