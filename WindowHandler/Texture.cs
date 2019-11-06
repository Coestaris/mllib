using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace WindowHandler
{
    public class Texture : IDisposable
    {
        public int ID;

        public Size Size;
        public string FileName;

        private bool _disposed;

        public Texture(Bitmap bitmap)
        {
            InitGLTexture(bitmap);
        }

        public Texture(string fileName)
        {
            FileName = fileName;
            if(!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            InitGLTexture(new Bitmap(fileName));
        }

        private void InitGLTexture(Bitmap bitmap)
        {
            int tex;
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            ID = tex;
            Size = bitmap.Size;
        }

        public void Dispose()
        {
            if(!_disposed)
                GL.DeleteTextures(1, ref ID);
            _disposed = true;
        }

        ~Texture()
        {
            if(!_disposed)
                Console.WriteLine("[Warning] Resource leaked: {0}.", typeof(Texture));
        }
    }
}