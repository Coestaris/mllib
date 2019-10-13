using OpenTK.Graphics;

namespace NNVisualizer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var win = new Window(800, 600, "NNVisualizer")
            {
                BackgroundColor = Color4.DarkBlue
            };

            WindowHandler handler = new NNVisualizer(win, null);
            handler.Start();
        }
    }
}