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

        private const int BrushSize = 20;
        private const double BrashFade = 3;

        private double[,] _data;
        private int[] PBOs;
        private int _index;
        private readonly int dataSize;
        private Random _random = new Random();
        private int _frameCounter;
        private double[,] _brush;
        private PointF _lastImagePoint;
        private bool _lastPoint;

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
            {
                var dist = Math.Sqrt(
                               (BrushSize / 2.0 - x) * (BrushSize / 2.0 - x) +
                               (BrushSize / 2.0 - y) * (BrushSize / 2.0 - y));
                var k = (Math.Log(-dist + BrushSize / 2.0 + 3) * (BrashFade / Math.E));
                if (double.IsNaN(k)) k = 0;
                if (k > 1) k = 1;
                if (k < 0) k = 0;
                //k = 1 - k;

                _brush[x, y] = (dist > BrushSize / 2.0 ? 0 : 1) * k;

                if (x == 10)
                    Console.WriteLine("dist {0:F3} k {1:F3} r {1:F3}", dist, k, _brush[x, y]);
            }
        }

        public override bool CheckMousePosition(Vector2 mouse)
        {
            return mouse.X > Position.X &&
                   mouse.X < Position.X + Size &&
                   mouse.Y > Position.Y &&
                   mouse.Y < Position.Y + Size;
        }

        private void Apply(PointF image)
        {
            for (var brushX = 0; brushX < BrushSize; brushX++)
            {
                var x = image.X + -BrushSize / 2.0 + brushX;
                for (var brushY = 0; brushY < BrushSize; brushY++)
                {
                    var y = image.Y + -BrushSize / 2.0 + brushY;
                    if (x < 0 || y < 0 || x >= Size || y >= Size) continue;

                    _data[(int) x, (int) y] += _brush[brushX, brushY];
                }
            }
        }

        private void Interpolate(double x, double y, double x2, double y2)
        {
            var w = x2 - x;
            var h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }

            var numerator = longest * 2;
            for (int i = 0; i <= longest; i++)
            {
                Apply(new PointF((float)x, (float)y));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        public override void OnMouseHover(MouseState ms)
        {
            if (ms.IsAnyButtonDown)
            {
                var client = Parent.PointToClient(new Point(ms.X, ms.Y));
                var image = new PointF(client.X - Position.X, client.Y - Position.Y);

                if (_lastPoint)
                {
                    Interpolate(
                        _lastImagePoint.X, _lastImagePoint.Y,
                        image.X, image.Y);
                }

                _lastImagePoint = image;
                _lastPoint = true;
            }
            else
            {
                _lastPoint = false;
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
            var ptr = (byte*) GL.MapBuffer(BufferTarget.PixelUnpackBuffer, BufferAccess.WriteOnly);
            if (ptr != null)
            {
                UpdatePixels(ptr);
                GL.UnmapBuffer(BufferTarget.PixelUnpackBuffer);
            }

            GL.BindBuffer(BufferTarget.PixelUnpackBuffer, 0);
        }

        public override void Draw()
        {
            if (_frameCounter % 2 == 0)
                UpdateTexture();

            DrawTexture(Texture, Position);

            _frameCounter++;
        }

        public void Reset()
        {
            for (var i = 0; i < Size; i++)
            for (var j = 0; j < Size; j++)
                _data[i, j] = 0;
        }
    }
}