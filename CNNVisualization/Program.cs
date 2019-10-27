using ml.AI.CNN;
using OpenTK.Graphics;
using WindowHandler;

namespace CNNVisualization
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var network = JSONParser.Parse("net.json");
            var window = new Window(900, 900, "CNNVisualization")
            {
                BackgroundColor = new Color4(94f / 255f, 91f / 255f, 102f / 255f, 0)
            };
            var handler = new Visualizer(window, network);

            handler.Start();
        }
    }
}