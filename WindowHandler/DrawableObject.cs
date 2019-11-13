using System;
using System.Drawing;
using System.Globalization;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MLLib.WindowHandler
{
    public abstract class DrawableObject
    {
        private bool _lastHover;
        private bool _lastClick;

        public Window Parent;

        public Vector2 Position;

        public abstract void Draw();

        public virtual bool CheckMousePosition(Vector2 mouse)
        {
            return false;
        }

        public virtual void Update()
        {
            MouseState ms = Mouse.GetState();
            var fixedPos = Parent.PointToClient(new Point(ms.X, ms.Y));
            if (CheckMousePosition(new Vector2(fixedPos.X, fixedPos.Y)))
            {
                if (!_lastHover)
                {
                    OnMouseHoverChanged(ms, true);
                    _lastHover = true;
                }

                if (ms.LeftButton == ButtonState.Pressed ||
                    ms.RightButton == ButtonState.Pressed ||
                    ms.MiddleButton == ButtonState.Pressed)
                {
                    if (!_lastClick)
                    {
                        OnMouseClickChanged(ms, true);
                        _lastClick = true;
                    }
                    OnMouseClick(ms);
                }
                else
                {
                    if (_lastClick)
                    {
                        OnMouseClickChanged(ms, false);
                        _lastClick = false;
                    }
                }
                OnMouseHover(ms);
            }
            else
            {
                if (_lastHover)
                {
                    OnMouseHoverChanged(ms, false);
                    _lastHover = false;
                }
            }
        }

        public virtual void OnMouseClick(MouseState ms) {}
        public virtual void OnMouseClickChanged(MouseState ms, bool state) {}
        public virtual void OnMouseHover(MouseState ms) {}
        public virtual void OnMouseHoverChanged(MouseState ms, bool state) {}

        protected void DrawCenteredString(string str, StringRenderer renderer, bool x, bool y)
        {
            var text = str;
            var textSize = renderer.MeasureString(text);
            var offsetPoint = new Vector2(x ? textSize.Width / 2 : 0, y ? textSize.Height / 2 : 0);
            renderer.DrawString(text, Position - offsetPoint);
        }

        protected void DrawCenteredTexture(Texture texture, bool x, bool y)
        {
            float offX = x ? texture.Size.Width / 2 : 0;
            float offY = y ? texture.Size.Height / 2 : 0;

            DrawTexture(texture, Position.X - offX, Position.Y - offY);
        }

        protected static void DrawTexture(int texture, float posX, float posY, float sizeX, float sizeY)
        {
            //GL.Disable(EnableCap.Blend);
            GL.Color3(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(posX, posY);

            GL.TexCoord2(1, 0);
            GL.Vertex2(posX + sizeX, posY);

            GL.TexCoord2(1, 1);
            GL.Vertex2(posX + sizeX, posY + sizeY);

            GL.TexCoord2(0, 1);
            GL.Vertex2(posX, posY + sizeY);

            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);
            //GL.Enable(EnableCap.Blend);
        }

        protected static void DrawTexture(Texture texture, float posX, float posY)
        {
            DrawTexture(texture.ID, posX, posY, texture.Size.Width, texture.Size.Height);
        }

        protected static void DrawTexture(Texture texture, float posX, float posY, float scaleX, float scaleY, double rotation = 0)
        {
            if (Math.Abs(rotation) < 1e-2)
            {
                DrawTexture(texture.ID, posX, posY, texture.Size.Width * scaleX, texture.Size.Height * scaleY);
                return;
            }

            GL.PushMatrix(); //Save the current matrix.
            GL.Translate(posX, posY, 0);
            GL.Rotate(rotation, 0, 0, 1);

            //GL.Disable(EnableCap.Blend);
            GL.Color3(Color.White);
            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0, 0);
            GL.Vertex2(-texture.Size.Width / 2.0f * scaleX, -texture.Size.Height / 2.0f * scaleY);

            GL.TexCoord2(1, 0);
            GL.Vertex2(texture.Size.Width / 2.0f * scaleX, -texture.Size.Height / 2.0f * scaleY);

            GL.TexCoord2(1, 1);
            GL.Vertex2(texture.Size.Width / 2.0f * scaleX, texture.Size.Height / 2.0f * scaleY);

            GL.TexCoord2(0, 1);
            GL.Vertex2(-texture.Size.Width / 2.0f * scaleX, texture.Size.Height / 2.0f * scaleY);


            GL.End();

            GL.PopMatrix();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            //GL.Enable(EnableCap.Blend);
        }

        private static float clipValue(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;

            return v;
        }

        protected Color LerpColor(Color a, Color b, float k)
        {
            //k = (float)MLLib.AI.NNLayer.Sigmoid(k);
            return Color.FromArgb(
                0,
                (int) clipValue(a.R * k + b.R * (1 - k), 0, 255),
                (int) clipValue(a.G * k + b.G * (1 - k), 0, 255),
                (int) clipValue(a.B * k + b.B * (1 - k), 0, 255));
        }

        public void Destroy()
        {
            Parent.Objects.Remove(this);
        }

        public DrawableObject(Vector2 position)
        {
            Position = position;
        }
    }
}