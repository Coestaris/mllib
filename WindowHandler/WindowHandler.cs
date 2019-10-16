namespace WindowHandler
{
    public abstract class WindowHandler
    {
        public Window Window;
        public ResourceManager ResourceManager;

        protected void AddObject(DrawableObject obj)
        {
            obj.Parent = Window;
            Window.Objects.Add(obj);
        }

        public WindowHandler(Window window)
        {
            ResourceManager = new ResourceManager();
            Window.ResourceManager = ResourceManager;
            Window = window;
        }

        public virtual void OnUpdate() {}

        public virtual void OnClose()
        {
            ResourceManager.FreeAll();
        }

        public virtual void OnStart()
        {
            Window.UpdateFunc = OnUpdate;
            Window.CloseFunc = OnClose;
            Window.Run(60);
        }
    }
}