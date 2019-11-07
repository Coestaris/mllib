using System;
using System.Collections.Generic;
using System.Linq;
using FlappyBird.Objects;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using WindowHandler;

namespace FlappyBird
{
    public class Game : WindowHandler.WindowHandler
    {
        public Resources Resources;
        public Player Player;
        public Ground Ground;

        public const double Speed = 1.8;
        public List<Pipe> Pipes;

        private int couter = 0;

        public Game(Window window, Resources resources) : base(window)
        {
            Resources = resources;
            Resources.RegisterTextures(ResourceManager);
            Pipes = new List<Pipe>();
        }

        protected override void OnUpdate()
        {
            if (couter % 120 == 0)
            {
                Pipe pipe;
                InsertObject(1, pipe = new Pipe(Resources.Pipes[0], -Speed,
                    new Vector2(Window.Width + Resources.Pipes[0].Size.Width, 0)));
                Pipes.Add(pipe);
            }

            couter++;
            var collided = false;
            foreach (var pipe in Pipes)
                if (pipe.CheckCollision(Player))
                {
                    collided = true;
                    break;
                }

            collided |= Ground.CheckCollision(Player);

            if(collided)
                Reset();
        }

        public void Reset()
        {
            Window.Objects.Clear();
            Window.KeyBinds.Clear();
            Pipes.Clear();

            AddObject(new Background(Resources.Backgrounds[0], -Speed));
            AddObject(Player = new Player(Resources.Birds[0]));
            AddObject(Ground = new Ground(Resources.Base, -Speed));

            couter = 0;
            Window.KeyBinds.Add(Key.Space, Player.Flap);
        }

        protected override void OnStart()
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            Reset();

            base.OnStart();
        }
    }
}