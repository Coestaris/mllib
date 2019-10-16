using System;
using System.Drawing;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace XORCalculator.Objects
{
    public class Neuron : DrawableObject
    {
        public float Activation;
        public float Radius;

        internal StringRenderer _renderer;

        public Neuron(float radius, Vector2 position) : base(position)
        {
            Radius = radius;
        }

        public override void Draw()
        {
            const double angleDelta = 0.01;

            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(lerpColor(Color.Red, Color.Green, Activation));
            GL.Vertex2(Position);
            for (double a = 0; a < Math.PI * 2; a += angleDelta)
                GL.Vertex2(
                    Position.X + Math.Cos(a) * Radius,
                    Position.Y + Math.Sin(a) * Radius);

            GL.End();

            GL.LineWidth(1);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Color3(Color.Black);
            for (double a = 0; a <= Math.PI * 2; a += angleDelta)
            {
                GL.Vertex2(
                    Position.X + Math.Cos(a) * Radius,
                    Position.Y + Math.Sin(a) * Radius);
            }

            GL.End();

            DrawCenteredString(Activation.ToString("F3"), _renderer, true, true);
        }
    }
}