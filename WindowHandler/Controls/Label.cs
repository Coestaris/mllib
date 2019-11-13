using OpenTK;

namespace MLLib.WindowHandler.Controls
{
    public class Label : DrawableObject
    {
        public string Text;
        private StringRenderer _renderer;

        public Label(string text, StringRenderer renderer, Vector2 position) : base(position)
        {
            Text = text;
            _renderer = renderer;
        }

        public override void Draw()
        {
            _renderer.DrawString(Text, Position);
        }
    }
}