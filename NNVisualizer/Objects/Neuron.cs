using System;
using System.Drawing;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NNVisualizer
{
    public class Neuron : DrawableObject
    {
        public float Weight;
        public float Radius;

        internal StringRenderer _renderer;

        public Neuron(float radius, Vector2 position) : base(position)
        {
            Radius = radius;
        }

        public override void Draw()
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(Color.Beige);

            GL.Vertex2(Position);
            for (double a = 0; a < Math.PI * 2; a += 0.01)
                GL.Vertex2(
                    Position.X + Math.Cos(a) * Radius,
                    Position.Y + Math.Sin(a) * Radius);

            GL.End();

            var text = Weight.ToString(CultureInfo.InvariantCulture);
            var textSize = _renderer.MeasureString(text);
            var offsetPoint = new Vector2(textSize.Width / 2, textSize.Height / 2);

            _renderer.DrawString(text, Position - offsetPoint);
        }
    }
}