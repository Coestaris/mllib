using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace WindowHandler
{
    public abstract class DrawableObject
    {
        private bool _lastHover;
        private bool _lastClick;

        public Window Parent;

        public Vector2 Position;

        public abstract void Draw();

        public virtual bool CheckMousePosition(Vector2 mouse)
        {
            return false;
        }

        public virtual void Update()
        {
            MouseState ms = Mouse.GetState();
            var fixedPos = Parent.PointToClient(new Point(ms.X, ms.Y));
            if (CheckMousePosition(new Vector2(fixedPos.X, fixedPos.Y)))
            {
                if (!_lastHover)
                {
                    OnMouseHoverChanged(ms, true);
                    _lastHover = true;
                }

                if (ms.LeftButton == ButtonState.Pressed ||
                    ms.RightButton == ButtonState.Pressed ||
                    ms.MiddleButton == ButtonState.Pressed)
                {
                    if (!_lastClick)
                    {
                        OnMouseClickChanged(ms, true);
                        _lastClick = true;
                    }
                    OnMouseClick(ms);
                }
                else
                {
                    if (_lastClick)
                    {
                        OnMouseClickChanged(ms, false);
                        _lastClick = false;
                    }
                }
                OnMouseHover(ms);
            }
            else
            {
                if (_lastHover)
                {
                    OnMouseHoverChanged(ms, false);
                    _lastHover = false;
                }
            }
        }

        public virtual void OnMouseClick(MouseState ms) {}
        public virtual void OnMouseClickChanged(MouseState ms, bool state) {}
        public virtual void OnMouseHover(MouseState ms) {}
        public virtual void OnMouseHoverChanged(MouseState ms, bool state) {}

        public DrawableObject(Vector2 position)
        {
            Position = position;
        }
    }
}