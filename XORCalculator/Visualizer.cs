using System;
using System.Drawing;
using System.Threading;
using ml.AI;
using OpenTK;
using XORCalculator.Objects;
using WindowHandler;
using WindowHandler.Controls;

namespace XORCalculator
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

        public bool _working;

        private const int StepsPerFrame = 2;
        private const int ErrorResetFactor = 0;
        private const int StepsDelay = 0;

        private Action _resetFunc;

        public NNVisualizer(Window window, NeuralNetwork network, Teacher teacher, Action ResetFunc) : base(window)
        {
            _resetFunc = ResetFunc;
            Network = network;
            Teacher = teacher;
        }

        public override void Update()
        {
            if (_working)
            {
                for (var i = 0; i < StepsPerFrame; i++)
                {
                    Step();
                    Thread.Sleep(StepsDelay);
                }
            }
        }

        private void Step()
        {
            Teacher.TeachStep(Network);

            DisplayValues();

            if (ErrorResetFactor != 0 && _infoRenderer.Step % ErrorResetFactor == 0)
                Teacher.ResetError();

            _infoRenderer.Error = Teacher.Error;
            _infoRenderer.Step++;
        }

        private void Reset()
        {
            Teacher.ResetError();
            _resetFunc();
            DisplayValues();

            _infoRenderer.Error = Teacher.Error;
            _infoRenderer.Step = 0;
        }

        private void DisplayValues()
        {
            for(var l = 0; l < Network.Layers.Count; l++)
            for (var n = 0; n < Network.Layers[l].Size; n++)
                _neurons[l][n].Activation = (float)Network.Layers[l].Activations[n];

            for(var l = 0; l < Network.Layers.Count - 1; l++)
            {
                var layer = Network.Layers[l];
                var nextLayer = Network.Layers[l + 1];
                for (var n = 0; n < layer.Size; n++)
                {
                    for (var j = 0; j < nextLayer.Size; j++)
                        _axons[l][n * nextLayer.Size + j].Weight =
                            (float)Network.Layers[l].Weights[n * nextLayer.Size + j];
                }
            }
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
                () => _working = true, _buttonStringRenderer, "Start"));

            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(185, 30),
                    () => _working = false, _buttonStringRenderer, "Stop"));


            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(310, 30),
                    () => Reset(), _buttonStringRenderer, "Reset"));

            AddObject(
                new Button(ButtonActiveTexture, ButtonTexture, new Vector2(435, 30),
                    () => Step(), _buttonStringRenderer, "Step"));

            _infoRenderer = new InfoRenderer(_textRenderer, Vector2.One);
            AddObject(_infoRenderer);

            Reset();

            base.Start();
        }
    }
}