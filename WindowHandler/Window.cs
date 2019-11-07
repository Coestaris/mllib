using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace WindowHandler
{
    public class Window : GameWindow
    {
        public List<DrawableObject> Objects;
        public Color4 BackgroundColor;

        public Action UpdateFunc;
        public Action CloseFunc;

        public Dictionary<Key, Action> KeyBinds;
        public static ResourceManager ResourceManager;

        public Window(int width, int height, string title) : base(width, height,
            new GraphicsMode(32, 24, 0, 8), title)
        {
            Objects = new List<DrawableObject>();
            KeyBinds = new Dictionary<Key, Action>();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            foreach (var keyBind in KeyBinds)
                if (input.IsKeyDown(keyBind.Key))
                    keyBind.Value();

            for(var i = 0; i < Objects.Count; i++)
                Objects[i].Update();

            UpdateFunc();
            base.OnUpdateFrame(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            CloseFunc();
            base.OnClosed(e);
        }

        public override void Exit()
        {
            CloseFunc();
            base.Exit();
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

            for(var i = 0; i < Objects.Count; i++)
                Objects[i].Draw();

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}