using ml.AIMath;

namespace ml.AI.CNN
{
    public class Kernel
    {
        public Matrix Matrix;
        public int SourceImageIndex;

        public Kernel(Matrix matrix, int sourceImageIndex)
        {
            Matrix = matrix;
            SourceImageIndex = sourceImageIndex;
        }
    }
}