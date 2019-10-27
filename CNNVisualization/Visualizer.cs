using System.Drawing;
using ml.AI;
using ml.AI.CNN;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WindowHandler;

namespace CNNVisualization
{
    public class Visualizer : WindowHandler.WindowHandler
    {
        public ConvolutionalNeuralNetwork Network;
        public Texture[][] Textures;

        public const float DefaultScale = 2.5f;

        public Visualizer(Window window, ConvolutionalNeuralNetwork network) : base(window)
        {
            Network = network;
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnStart()
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);

            var input = new Bitmap("img3.png");
            var inputVolume = new Volume(input);

            var ax = Window.Width / Network.Layers.Count;
            var dx = ax - ax / (float)Network.Layers.Count;
            var scale = DefaultScale;

            var id = 0;
            Network.ForwardPass(inputVolume);
            Textures = new Texture[Network.Layers.Count][];
            for (var i = 0; i < Network.Layers.Count; i++)
            {
                Textures[i] = new Texture[Network.Layers[i].OutDepth];

                if (Network.Layers[i].OutSize.Width == 1 &&
                    Network.Layers[i].OutSize.Height == 1) scale = DefaultScale * 2;
                else scale = DefaultScale;

                var dy = Window.Height / 2.0f -
                         (Network.Layers[i].OutSize.Height * scale / 2) / Network.Layers[i].OutDepth;
                for (int j = 0; j < Network.Layers[i].OutDepth; j++)
                {
                    var bmp = Network.Layers[i].ToBitmap(j, Color.AntiqueWhite, Color.Black);
                    var tex = new Texture(bmp);

                    ResourceManager.PushTexture(id++, tex);
                    Textures[i][j] = tex;

                    var obj = new Picture(
                        new Vector2(
                            ax + dx * i,
                            (float)(dy + (j - Network.Layers[i].OutDepth / 2) * (Network.Layers[i].OutSize.Height * scale / 2 * 2.5))),
                        tex, new Vector2(scale, scale));
                    AddObject(obj);
                }
            }
        }
    }
}