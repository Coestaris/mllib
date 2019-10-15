using System.Drawing;
using ml.AI;
using OpenTK;

namespace NNVisualizer
{
    public class NNVisualizer : WindowHandler
    {
        public NeuralNetwork Network;
        internal StringRenderer _neuronStringRenderer;

        private Neuron[][] _neurons;
        private Axon[][] _axons;

        public NNVisualizer(Window window, NeuralNetwork network) : base(window)
        {
            Network = network;
            Window.UpdateFunc = Update;
        }

        public void Update()
        {

        }

        public override void Start()
        {
            _neuronStringRenderer = new StringRenderer(
                StringRenderer.NumericCharSet,
                new Font(FontFamily.GenericSerif, 12),
                Brushes.Black);

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

            base.Start();
        }
    }
}