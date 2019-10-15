namespace WindowHandler
{
    public abstract class WindowHandler
    {
        public Window Window;

        protected void AddObject(DrawableObject obj)
        {
            obj.Parent = Window;
            Window.Objects.Add(obj);
        }

        public WindowHandler(Window window)
        {
            Window = window;
        }

        public virtual void Start()
        {
            Window.Run(60);
        }
    }
}