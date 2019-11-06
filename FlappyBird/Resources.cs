using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageMagick;
using WindowHandler;

namespace FlappyBird
{
    public class Resources
    {
        public string DirName = "sprites/";
        public string DirPath = "../../";

        public Dictionary<int, Texture> FontTextures;
        public Texture Base;
        public Texture Gameover;
        public Texture Tutorial;
        public Texture[] Pipes;
        public Texture[] Backgrounds;
        public Texture[][] Birds;

        private Bitmap LoadBitmap(string filename)
        {
            using (var image = new MagickImage(filename))
                return image.ToBitmap();
        }

        public Resources()
        {
            if (!Directory.Exists(DirName))
            {
                var srcName = DirPath + DirName;
                if (!Directory.Exists(srcName))
                    throw new DirectoryNotFoundException(srcName);

                Directory.CreateDirectory(DirName);

                foreach (string newPath in Directory.GetFiles(srcName, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(srcName, DirName), true);
            }

            DirName = Path.GetFullPath(DirName);
            FontTextures = new Dictionary<int, Texture>();
            for(int i = 0; i <= 9; i++)
                FontTextures.Add(i, new Texture(LoadBitmap($"{DirName}{i}.png")));

            Backgrounds = new[]
            {
                new Texture(LoadBitmap($"{DirName}background-day.png")),
                new Texture(LoadBitmap($"{DirName}background-night.png"))
            };

            Base = new Texture(LoadBitmap($"{DirName}base.png"));
            Gameover = new Texture(LoadBitmap($"{DirName}gameover.png"));
            Tutorial = new Texture(LoadBitmap($"{DirName}message.png"));

            Pipes = new[]
            {
                new Texture(LoadBitmap($"{DirName}pipe-green.png")),
                new Texture(LoadBitmap($"{DirName}pipe-red.png"))
            };

            Birds = new Texture[3][];
            Birds[0] = new[]
            {
                new Texture(LoadBitmap($"{DirName}redbird-downflap.png")),
                new Texture(LoadBitmap($"{DirName}redbird-midflap.png")),
                new Texture(LoadBitmap($"{DirName}redbird-upflap.png"))
            };

            Birds[1] = new[]
            {
                new Texture(LoadBitmap($"{DirName}yellowbird-downflap.png")),
                new Texture(LoadBitmap($"{DirName}yellowbird-midflap.png")),
                new Texture(LoadBitmap($"{DirName}yellowbird-upflap.png"))
            };

            Birds[2] = new[]
            {
                new Texture(LoadBitmap($"{DirName}bluebird-downflap.png")),
                new Texture(LoadBitmap($"{DirName}bluebird-midflap.png")),
                new Texture(LoadBitmap($"{DirName}bluebird-upflap.png"))
            };
        }

        public void RegisterTextures(ResourceManager manager)
        {
            foreach (var texture in FontTextures)
                manager.PushTexture(texture.Value.ID, texture.Value);
            foreach (var background in Backgrounds)
                manager.PushTexture(background.ID, background);
            foreach (var pipe in Pipes)
                manager.PushTexture(pipe.ID, pipe);
            foreach (var array in Birds)
            foreach (var texture in array)
                manager.PushTexture(texture.ID, texture);

            manager.PushTexture(Tutorial.ID, Tutorial);
            manager.PushTexture(Gameover.ID, Gameover);
            manager.PushTexture(Base.ID, Base);
        }
    }
}