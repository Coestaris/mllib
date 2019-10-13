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

        private TextRenderer _renderer;
        private int _texture;

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

            GL.ClearColor(BackgroundColor);

            _renderer = new TextRenderer(200, 200);
            _renderer.DrawString("123456789", new Font(FontFamily.GenericSansSerif, 16), Brushes.Peru, new PointF(0, 0));
            _texture = _renderer.Texture;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var obj in Objects)
                obj.Draw();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(100f, -1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(100f, 100f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, 100f);

            GL.End();
            //DrawFunc();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}