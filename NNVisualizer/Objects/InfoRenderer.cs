using ml.AI;
using OpenTK;
using WindowHandler;

namespace NNVisualizer
{
    public class InfoRenderer : DrawableObject
    {
        internal StringRenderer _renderer;

        internal int Step;
        internal double Error;

        internal Teacher Teacher;

        public InfoRenderer(StringRenderer renderer, Vector2 position) : base(position)
        {
            _renderer = renderer;
        }

        public override void Draw()
        {
            _renderer.DrawString("Step: " + Step, new Vector2(5, 50));
            _renderer.DrawString("Error: " + Error.ToString("F2"), new Vector2(5, 70));
        }
    }
}