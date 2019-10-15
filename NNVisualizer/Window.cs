using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NNVisualizer
{
    public class Window : GameWindow
    {
        public List<DrawableObject> Objects;
        public Color4 BackgroundColor;

        public Action UpdateFunc;
        public Action DrawFunc;

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

            foreach (var drawableObject in Objects)
                drawableObject.Update();

            //UpdateFunc();
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

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(BackgroundColor);

            foreach (var obj in Objects)
                obj.Draw();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}