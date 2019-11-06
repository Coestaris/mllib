using OpenTK;
using WindowHandler;

namespace FlappyBird.Objects
{
    public class Player : DrawableObject
    {
        private Texture[] _playerTextures;
        private int       _textureCounter = 0;
        private double    _playerRot     = 45;
        private double    _playerVelRot  = 3;

        private double    _playerVelY    = -9;
        private bool      _playerFlapped = false;

        private const double PlayerMaxVelY = 10;
        private const double PlayerMinVelY = -8;
        private const double PlayerAccY    = 1;

        private const double PlayerFlapAcc = -9;
        private const double PlayerRotThr  = 20;

        public Player(Texture[] playerTextures) : base(new Vector2(0, 100))
        {
            _playerTextures = playerTextures;
        }

        public override void Update()
        {
            if (_playerVelY < PlayerMinVelY && !_playerFlapped)
                _playerVelY -= PlayerAccY;

            //Position.Y -= (float)_playerVelY;
        }

        public override void Draw()
        {
            DrawTexture(_playerTextures[_textureCounter++ % _playerTextures.Length], Position);
        }
    }
}