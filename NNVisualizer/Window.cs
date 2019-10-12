using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NNVisualizer
{
    internal class Window : GameWindow
    {
        public List<DrawableObject> Objects;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            Objects = new List<DrawableObject>();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            base.OnUpdateFrame(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            //setup matrices
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(0, Width, Height, 0, -1, 1);
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Modelview);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            Objects.Add(new Circle(new Vector2(20, 20), new Vector3(1, 1, 1), 100));
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var obj in Objects)
                obj.Draw();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}