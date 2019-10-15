using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace NNVisualizer
{
    public class Axon : DrawableObject
    {
        public Vector2 Position2;
        public float Weight;

        public Axon(Vector2 position, Vector2 position2, float weight) : base(position)
        {
            Position2 = position2;
            Weight = weight;
        }

        public override void Draw()
        {
            GL.LineWidth(Weight);
            GL.Color3(Color.White);

            GL.Begin(BeginMode.Lines);

            GL.Vertex2(Position);
            GL.Vertex2(Position2);

            GL.End();

        }
    }
}