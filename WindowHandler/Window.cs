using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MLLib.WindowHandler
{
    public class Window : GameWindow
    {
        public List<DrawableObject> Objects;
        public WindowHandler Handler;
        public Color4 BackgroundColor;

        public Action UpdateFunc;
        public Action CloseFunc;

        public Dictionary<Key, Action> KeyDownBinds;
        public Dictionary<Key, Action> KeyUpBinds;
        public Dictionary<MouseButton, Action> MouseDownBind;
        public Dictionary<MouseButton, Action> MouseUpBind;
        public static ResourceManager ResourceManager;

        public Window(int width, int height, string title) : base(width, height,
            new GraphicsMode(32, 24, 0, 8), title)
        {
            Objects = new List<DrawableObject>();
            KeyDownBinds = new Dictionary<Key, Action>();
            KeyUpBinds = new Dictionary<Key, Action>();
            MouseDownBind = new Dictionary<MouseButton, Action>();
            MouseUpBind = new Dictionary<MouseButton, Action>();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();
            var mouse = Mouse.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            foreach (var keyBind in KeyDownBinds)
                if (input.IsKeyDown(keyBind.Key))
                    keyBind.Value();

            foreach (var keyBind in KeyUpBinds)
                if (input.IsKeyDown(keyBind.Key))
                    keyBind.Value();

            foreach (var keyBind in MouseDownBind)
                if (mouse.IsButtonDown(keyBind.Key))
                    keyBind.Value();

            foreach (var keyBind in MouseUpBind)
                if (mouse.IsButtonUp(keyBind.Key))
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