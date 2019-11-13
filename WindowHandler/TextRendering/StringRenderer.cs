using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MLLib.WindowHandler
{
    public class StringRenderer
    {
        private struct CharInfo
        {
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;
            public Vector2 TexCoord3;
            public Vector2 TexCoord4;

            public SizeF CharSize;
        }

        internal readonly TextRenderer _renderer;
        private readonly int _texture;
        private readonly Dictionary<char, CharInfo> _charInfos;

        public readonly string CharSet;
        public readonly Font Font;
        public readonly Brush Brush;

        public static string NumericCharSet = "0123456789.";
        public static string FullCharSet
        {
            get
            {
                var s = "";
                for (int i = 30; i < 127; i++)
                    s += (char)i;
                return s;
            }
        }

        public StringRenderer(string charSet, Font font, Brush brush)
        {
            if(charSet == null)
                throw new ArgumentNullException(nameof(charSet));
            if(font == null)
                throw new ArgumentNullException(nameof(font));

            CharSet = charSet;
            Font = font;
            Brush = brush;

            var charSetSize = TextRenderer.MeasureString(charSet, font);
            _renderer = new TextRenderer((int)charSetSize.Width, (int)charSetSize.Height);
            _renderer.DrawString(charSet, font, brush, new PointF(0, 0));
            _texture = _renderer.Texture;

            _charInfos = new Dictionary<char, CharInfo>();
            var xOff = 0.0f;
            foreach (var c in charSet)
            {
                var ci = new CharInfo();
                var charSize = TextRenderer.MeasureString(c.ToString(), font);
                var nextX = xOff + charSize.Width;

                ci.TexCoord1 = new Vector2(xOff / charSetSize.Width, 1);
                ci.TexCoord2 = new Vector2(nextX / charSetSize.Width, 1);
                ci.TexCoord3 = new Vector2(nextX / charSetSize.Width, 0);
                ci.TexCoord4 = new Vector2(xOff / charSetSize.Width, 0);
                ci.CharSize = charSize;

                _charInfos.Add(c, ci);
                xOff = nextX;
            }

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
        }


        public void DrawString(string str, Vector2 pos)
        {
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            var xOff = pos.X;
            var y1 = pos.Y;
            var y2 = pos.Y + Font.GetHeight();
            foreach (var c in str)
            {
                var ci = _charInfos[c];
                var xNext = xOff + ci.CharSize.Width;
                GL.Begin(BeginMode.Quads);
                {
                    GL.TexCoord2(ci.TexCoord1);
                    GL.Vertex2(xOff, y2);
                    GL.TexCoord2(ci.TexCoord2);
                    GL.Vertex2(xNext, y2);
                    GL.TexCoord2(ci.TexCoord3);
                    GL.Vertex2(xNext, y1);
                    GL.TexCoord2(ci.TexCoord4);
                    GL.Vertex2(xOff, y1);
                }
                GL.End();
                xOff = xNext;
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public SizeF MeasureString(string text)
        {
            return TextRenderer.MeasureString(text, Font);
        }
    }
}