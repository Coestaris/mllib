using System;
using System.Drawing;
using ml.AIMath;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace FlappyBird.Objects
{
    public class Pipe : DrawableObject
    {
        private static Random _random = new Random();

        private Texture _texture;
        private double _speed;

        private double _x;
        private double _yOffset;

        private const float PipeWidth = 120;

        public RectangleF Rectangle1;
        public RectangleF Rectangle2;

        public Pipe(Texture texture, double speed, Vector2 position) : base(position)
        {
            _texture = texture;
            _speed = speed;

            _yOffset = _random.Next(-10, 140);
            _x = position.X;
        }

        public override void Update()
        {
            _x += (float)_speed;
            if (_x < -_texture.Size.Width)
                Destroy();

            Rectangle1.X = (float) _x - _texture.Size.Width / 2.0f;
            Rectangle1.Y = 0;
            Rectangle1.Width = _texture.Size.Width;
            Rectangle1.Height = Parent.Height / 2.0f - (float) _yOffset - PipeWidth;

            Rectangle2.X = (float) _x - _texture.Size.Width / 2.0f;
            Rectangle2.Y = Parent.Height / 2.0f - (float) _yOffset + PipeWidth;
            Rectangle2.Width = _texture.Size.Width;
            Rectangle2.Height = Parent.Height / 2.0f;

        }

        internal static void DrawRectangle(RectangleF rectangle, Color color)
        {
            GL.LineWidth(3);
            GL.Color3(color);

            GL.Begin(BeginMode.LineStrip);

            GL.Vertex2(rectangle.Location.X, rectangle.Location.Y);
            GL.Vertex2(rectangle.Location.X, rectangle.Location.Y + rectangle.Height);

            GL.Vertex2(rectangle.Location.X, rectangle.Location.Y + rectangle.Height);
            GL.Vertex2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y + rectangle.Height);

            GL.Vertex2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y + rectangle.Height);
            GL.Vertex2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y);

            GL.Vertex2(rectangle.Location.X + rectangle.Width, rectangle.Location.Y);
            GL.Vertex2(rectangle.Location.X, rectangle.Location.Y);

            GL.End();
        }

        public override void Draw()
        {
            DrawTexture(
                _texture,
                (float) _x - _texture.Size.Width / 2.0f,
                Parent.Height / 2.0f + PipeWidth - (float) _yOffset,
                1,
                1);


            DrawTexture(
                _texture,
                (float) _x,
                Parent.Height / 2.0f - PipeWidth - (float) _yOffset - _texture.Size.Height / 2.0f,
                -1,
                1,
                180);

            //DrawRectangle(Rectangle1, Color.Blue);
            //DrawRectangle(Rectangle2, Color.Purple);
        }

        public bool CheckCollision(Player player)
        {
            return Rectangle1.IntersectsWith(player.Rectangle) || Rectangle2.IntersectsWith(player.Rectangle);
        }
    }
}