using System;
using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace MLLib.WindowHandler.Controls
{
    public class Checkbox : DrawableObject
    {
        public Texture Texture;
        public Texture ActiveTexture;
        public Texture CheckedTexture;
        public Texture CheckedActiveTexture;

        public string Text;

        private StringRenderer _renderer;
        private bool _checkState;
        private bool _hovered;

        private Action<bool> _changedValue;

        public bool Checked => _checkState;

        public Checkbox(
            string text,
            int texture, int activeTexture,
            int checkedTexture, int checkedActiveTexture,
            Vector2 position,
            Action<bool> changedValue,
            StringRenderer renderer) : base(position)
        {
            Text = text;
            Texture = Window.ResourceManager.Get(texture);
            ActiveTexture = Window.ResourceManager.Get(activeTexture);
            CheckedTexture = Window.ResourceManager.Get(checkedTexture);
            CheckedActiveTexture = Window.ResourceManager.Get(checkedActiveTexture);
            _checkState = false;
            _changedValue = changedValue;

            _renderer = renderer;
        }

        private Texture getCurrentTexture()
        {
            if (_checkState)
                return _hovered ? CheckedActiveTexture : CheckedTexture;
            else
                return _hovered ? ActiveTexture : Texture;
        }

        public override bool CheckMousePosition(Vector2 mouse)
        {
            var currentTexture = getCurrentTexture();
            return mouse.X > Position.X - currentTexture.Size.Width / 2.0 &&
                   mouse.X < Position.X + currentTexture.Size.Width / 2.0 &&
                   mouse.Y > Position.Y - currentTexture.Size.Height / 2.0 &&
                   mouse.Y < Position.Y + currentTexture.Size.Height / 2.0;
        }

        public override void OnMouseHoverChanged(MouseState ms, bool state)
        {
            _hovered = state;
        }

        public override void OnMouseClickChanged(MouseState ms, bool state)
        {
            if (state)
            {
                _checkState = !_checkState;
                _changedValue(_checkState);
            }
        }

        public override void Draw()
        {
            var currentTexture = getCurrentTexture();
            DrawCenteredTexture(currentTexture, true, true);

            var textSize = TextRenderer.MeasureString(Text, _renderer.Font);
            _renderer.DrawString(Text, Position +
                    new Vector2(currentTexture.Size.Width / 2.0f + 10, -textSize.Height / 2.0f));
        }
    }
}