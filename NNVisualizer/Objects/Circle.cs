using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NNVisualizer
{
    internal class Circle : DrawableObject
    {
        public double Radius;

        public Circle(Vector2 position, Vector3 color, double radius) : base(position, color)
        {
            Radius = radius;
        }

        public override void Draw()
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(Color);

            GL.Vertex2(Position);
            for (double a = 0; a < Math.PI * 2; a += 0.01)
                GL.Vertex2(
                    Position.X + Math.Cos(a) * Radius,
                    Position.Y + Math.Sin(a) * Radius);

            GL.End();
        }
    }
}