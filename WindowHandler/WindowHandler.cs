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

        protected virtual void OnUpdate() {}

        protected virtual void OnClose()
        {
            ResourceManager.FreeAll();
        }

        protected virtual void OnStart() { }

        public void Start()
        {
            OnStart();
            Window.UpdateFunc = OnUpdate;
            Window.CloseFunc = OnClose;
            Window.Run(60);
        }
    }
}