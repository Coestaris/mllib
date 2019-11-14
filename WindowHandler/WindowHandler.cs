namespace MLLib.WindowHandler
{
    public abstract class WindowHandler
    {
        public Window Window;
        public ResourceManager ResourceManager;

        public DrawableObject InsertObject(int index, DrawableObject obj)
        {
            obj.Parent = Window;
            Window.Objects.Insert(index, obj);
            return obj;
        }

        public DrawableObject AddObject(DrawableObject obj)
        {
            obj.Parent = Window;
            Window.Objects.Add(obj);
            return obj;
        }

        public WindowHandler(Window window)
        {
            ResourceManager = new ResourceManager();
            Window.ResourceManager = ResourceManager;
            Window = window;
            window.Handler = this;
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