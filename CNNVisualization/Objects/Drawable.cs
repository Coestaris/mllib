using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using WindowHandler;

namespace CNNVisualization.Objects
{
    public class Drawable : DrawableObject
    {
        public Texture Texture;
        public int Size;

        private double[,] _data;
        private int[] PBOs;
        private int _index;
        private readonly int dataSize;
        private Random _random = new Random();
        private int _frameCounter;

        private const int BrushSize = 35;
        private double[,] _brush;

        public Drawable(Vector2 position, int size) : base(position)
        {
            Texture = new Texture(new Bitmap(size, size));
            _data = new double[size, size];
            Size = size;

            dataSize = size * size * 3;

            PBOs = new int[2];
            GL.GenBuffers(2, PBOs);

            for (var i = 0; i < PBOs.Length; i++)
            {
                GL.BindBuffer(BufferTarget.PixelUnpackBuffer, PBOs[i]);
                GL.BufferData(BufferTarget.PixelUnpackBuffer, dataSize, IntPtr.Zero, BufferUsageHint.StreamDraw);

            }
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);

            _brush = new double[BrushSize, BrushSize];
            for (var x = 0; x < BrushSize; x++)
            for (var y = 0; y < BrushSize; y++)
                _brush[x, y] = Math.Sqrt(
                                  (BrushSize / 2.0 - x) * (BrushSize / 2.0 - x) +
                                  (BrushSize / 2.0 - y) * (BrushSize / 2.0 - y)) > BrushSize / 2.0
                    ? 0
                    : 1;
        }

        public override bool CheckMousePosition(Vector2 mouse)
        {
            return mouse.X > Position.X &&
                   mouse.X < Position.X + Size &&
                   mouse.Y > Position.Y &&
                   mouse.Y < Position.Y + Size;
        }

        public override void OnMouseHover(MouseState ms)
        {
            if (ms.LeftButton == ButtonState.Pressed)
            {
                var client = Parent.PointToClient(new Point(ms.X, ms.Y));
                var image = new PointF(client.X - Position.X, client.Y - Position.Y);

                for (var brushX = 0; brushX < BrushSize; brushX++)
                {
                    var x = image.X + -BrushSize / 2.0 + brushX;
                    for (var brushY = 0; brushY < BrushSize; brushY++)
                    {
                        var y = image.Y + -BrushSize / 2.0 + brushY;
                        if(x < 0 || y < 0 || x > Size || y > Size) continue;

                        _data[(int) x, (int) y] += _brush[brushX, brushY];
                    }
                }
            }

            base.OnMouseHover(ms);
        }

        private byte NormilizeColor(double d)
        {
            var b = d * 255;
            if (b > 255) return 255;
            if (b < 0) return 0;
            return (byte) b;
        }

        private unsafe void UpdatePixels(byte* ptr)
        {
            for (var i = 0; i < dataSize; i += 3)
            {
                var index = i / 3;
                var b = NormilizeColor(_data[index % Size, index / Size]);
                ptr[i] = b;
                ptr[i + 1] = b;
                ptr[i + 2] = b;
            }
        }

        private unsafe void UpdateTexture()
        {
            var pbo1 = PBOs[(_index) % PBOs.Length];
            var pbo2 = PBOs[(_index + 1) % PBOs.Length];
            _index++;

            GL.BindTexture(TextureTarget.Texture2D, Texture.ID);
            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pbo1);

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0,
                Size, Size, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, pbo2);
            GL.BufferData(BufferTarget.PixelUnpackBuffer, dataSize, IntPtr.Zero, BufferUsageHint.StreamDraw);
            var ptr = (byte*)GL.MapBuffer(BufferTarget.PixelUnpackBuffer, BufferAccess.WriteOnly);
            if(ptr != null)
            {
                UpdatePixels(ptr);
                GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);
            }

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        public override void Draw()
        {
            if(_frameCounter % 5 == 0)
                UpdateTexture();

            DrawTexture(Texture, Position);

            _frameCounter++;
        }
    }
}