using System;
using OpenTK;
using OpenTK.Input;

namespace WindowHandler.Controls
{
    public class Button : DrawableObject
    {
        public Texture TextureActive;
        public Texture Texture;

        public Action Action;
        public string Text;
        private StringRenderer _renderer;

        private bool _hovered;

        public Button(
            Texture tex1,
            Texture tex2,
            Vector2 position,
            Action action,
            StringRenderer renderer,
            string text) : base(position)
        {
            TextureActive = tex1;
            Texture = tex2;
            Action = action;
            _renderer = renderer;
            Text = text;
        }

        public override bool CheckMousePosition(Vector2 mouse)
        {
            var currentTexture = _hovered ? TextureActive : Texture;
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
            if (state) Action();
        }

        public override void Draw()
        {
            DrawCenteredTexture(_hovered ? TextureActive : Texture, true, true);
            DrawCenteredString(Text, _renderer, true, true);
        }
    }
}