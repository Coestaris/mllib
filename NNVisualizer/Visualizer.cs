using System;
using System.Drawing;
using ml.AI;
using OpenTK;
using WindowHandler;
using WindowHandler.Controls;

namespace NNVisualizer
{
    public class NNVisualizer : WindowHandler.WindowHandler
    {
        public Texture ButtonActiveTexture;
        public Texture ButtonTexture;

        public NeuralNetwork Network;
        public Teacher Teacher;

        internal StringRenderer _neuronStringRenderer;
        internal StringRenderer _buttonStringRenderer;
        internal StringRenderer _textRenderer;

        private Neuron[][] _neurons;
        private Axon[][] _axons;
        private InfoRenderer _infoRenderer;

        public NNVisualizer(Window window, NeuralNetwork network, Teacher teacher) : base(window)
        {
            Network = network;
            Teacher = teacher;
        }

        public override void Update()
        {
            _infoRenderer.Step++;
        }

        public override void Start()
        {
            ButtonTexture = new Texture("button.png");
            ButtonActiveTexture = new Texture("buttonActive.png");

            _neuronStringRenderer = new StringRenderer(
                StringRenderer.NumericCharSet,
                new Font(FontFamily.GenericSerif, 12),
                Brushes.Black);

            _buttonStringRenderer = new StringRenderer(
                StringRenderer.FullCharSet,
                new Font(FontFamily.GenericMonospace, 16),
                Brushes.Black);

            _textRenderer = new StringRenderer(
                StringRenderer.FullCharSet,
                new Font(FontFamily.GenericMonospace, 20),
                Brushes.White);


            _neurons = new Neuron[Network.Layers.Count][];
            _axons   = new Axon  [Network.Layers.Count][];

            var xStep = Window.Width / (float)(Network.Layers.Count + 1);
            var x = xStep;
            var layerCount = 0;
            foreach (var layer in Network.Layers)
            {
                _neurons[layerCount] = new Neuron[layer.Size];
                if(layer.WeightsNudge != null)
                    _axons[layerCount] = new Axon[layer.WeightsNudge.Length];

                var yStep = Window.Height / (float) (layer.Size + 1);
                var y = yStep;

                for (var i = 0; i < layer.Size; i++)
                {
                    _neurons[layerCount][i] = new Neuron(30, new Vector2(x, y))
                    {
                        _renderer = _neuronStringRenderer
                    };
                    y += yStep;
                }

                x += xStep;
                layerCount++;
            }

            for(int l = 0; l < Network.Layers.Count - 1; l++)
            {
                var layer = Network.Layers[l];
                var nextLayer = Network.Layers[l + 1];

                for (var i = 0; i < layer.Size; i++)
                {
                    for (var j = 0; j < nextLayer.Size; j++)
                    {
                        _axons[l][i * nextLayer.Size + j] = new Axon(
                            _neurons[l][i].Position,
                            _neurons[l + 1][j].Position,
                            (float) layer.Weights[i * nextLayer.Size + j]
                        );
                        AddObject(_axons[l][i * nextLayer.Size + j]);
                    }
                }
            }

            foreach (var neurons in _neurons)
                foreach (var neuron in neurons)
                    AddObject(neuron);

            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(60, 30),
                () => Console.WriteLine("start"), _buttonStringRenderer, "Start"));

            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(185, 30),
                    () => Console.WriteLine("stop"), _buttonStringRenderer, "Stop"));


            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(310, 30),
                    () => Console.WriteLine("reset"), _buttonStringRenderer, "Reset"));

            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(435, 30),
                    () => Console.WriteLine("step"), _buttonStringRenderer, "Step"));

            _infoRenderer = new InfoRenderer(_textRenderer, Vector2.One);
            AddObject(_infoRenderer);

            base.Start();
        }
    }
}