using System.Drawing;
using OpenTK;
using WindowHandler;

namespace CNNVisualization.Objects
{
    public class Drawable : DrawableObject
    {
        public Texture Texture;

        public Drawable(Vector2 position, int size) : base(position)
        {
            Texture = new Texture(new Bitmap(size, size));
        }

        public override void Draw()
        {
            DrawTexture(Texture, Position);
        }
    }
}