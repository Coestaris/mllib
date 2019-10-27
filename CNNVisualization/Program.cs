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
            var window = new Window(1000, 700, "CNNVisualization");
            var handler = new Visualizer(window, network);
            handler.Start();

        }
    }
}