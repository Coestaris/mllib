using OpenTK;

namespace NNVisualizer
{
    public abstract class DrawableObject
    {
        public Vector2 Position;
        public Vector3 Color;

        public abstract void Draw();

        public DrawableObject(Vector2 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }

    }
}