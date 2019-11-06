using FlappyBird.Objects;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace FlappyBird
{
    public class Game : WindowHandler.WindowHandler
    {
        public Resources Resources;
        public Player Player;

        public Game(Window window, Resources resources) : base(window)
        {
            Resources = resources;
            Resources.RegisterTextures(ResourceManager);
        }

        protected override void OnStart()
        {
            AddObject(Player = new Player(Resources.Birds[0]));
            base.OnStart();
        }
    }
}