using System;
using System.Drawing;
using ml.AI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace XORCalculator.Objects
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
            GL.Color3(1, 1, 1);
            _renderer.DrawString("Step: " + Step, new Vector2(5, 50));
            _renderer.DrawString("Error: ", new Vector2(5, 70));

            if (double.IsNaN(Error))
            {
                _renderer.DrawString("none", new Vector2(100, 72));
            }
            else
            {
                GL.Color3(lerpColor(Color.Red, Color.Green, (float)Error));
                _renderer.DrawString(Error.ToString("F4"), new Vector2(100, 72));
            }
        }
    }
}