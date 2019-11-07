using System;
using OpenTK;
using WindowHandler;

namespace FlappyBird.Objects
{
    public class Player : DrawableObject
    {
        private Texture[] _playerTextures;

        private int    _textureCounter = 0;
        private int _frameCounter = 0;

        private double _rot     = 45;
        private double _rotVel  = 3;
        private double _yVel    = 9;

        private bool   _flapped = false;

        private const double MaxYVel  =  -12;
        private const double YAcc     =   -2;

        private const double FlapYVel =   12;
        private const double FLapRotVel = -16;

        private const double RotAcc   =     2;
        private const double RotMax   =    35;
        private const int AnimationSpeed =  4;

        public Player(Texture[] playerTextures) : base(new Vector2(200, 100))
        {
            _playerTextures = playerTextures;
            Flap();
        }

        public override void Update()
        {
            if (_yVel > MaxYVel && !_flapped)
                _yVel += YAcc;

            Position.Y -= (float)_yVel;

            if (_rot < RotMax)
            {
                _rotVel += RotAcc;
                _rot += _rotVel;
            }

            if (_rot > RotMax) _rot = RotMax;

            _frameCounter++;
            if (_frameCounter % AnimationSpeed == 0)
            {
                _textureCounter = (_textureCounter + 1) % _playerTextures.Length;
                _flapped = !_flapped;
            }


        }

        public override void Draw()
        {
            DrawTexture(_playerTextures[_textureCounter],
                Position,
                Vector2.One,
                _rot);
        }

        public void Flap()
        {
            _yVel = FlapYVel;

            _rot = RotMax - 3;
            _rotVel = FLapRotVel;
        }
    }
}