namespace ml.AIMath
{
    public class Size
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        protected bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }

        public static bool operator ==(Size a, Size b)
        {
            if ((object) a == null && (object) b == null)
                return true;

            if ((object) a == null || (object) b == null)
                return false;

            return a.Width == b.Width &&
                   a.Height == b.Height;
        }

        public static bool operator !=(Size a, Size b)
        {
            return !(a == b);
        }
    }
}