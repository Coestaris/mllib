using ml.AIMath;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var m1 = new Matrix(5, 5);
            m1.FillGaussianRandom();
            m1.Print();
        }
    }
}