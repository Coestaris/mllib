using System.Drawing;
using ml.AI.CNN;
using OpenTK;
using WindowHandler;

namespace CNNVisualization.Objects
{
    public class LayerThumb : DrawableObject
    {
        public Texture Texture;
        public Vector2 Scale;

        public CNNLayer Layer;
        public int Depth;

        public void RebuildTexture()
        {
            var bmp = Layer.ToBitmap(Depth, Color.Cornsilk, Color.Black);
            
            Texture =
        }

        public LayerThumb(Vector2 position, Vector2 scale, CNNLayer layer, int depth) : base(position)
        {
            Scale = scale;
            Layer = layer;
            Depth = depth;
        }

        public override void Draw()
        {
            //DrawTexture(Texture, Position, Scale);
        }
    }
}