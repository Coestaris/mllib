using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MLLib.AI.CNN.Layers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MLLib.AI.CNN
{
    /*
     *"sx": 5,
      "sy": 5,
      "out_sx": 24,
      "out_sy": 24,
      "in_depth": 1,
      "out_depth": 8,
      "relu", "conv", "input", "pool", "fc", "softmax"


      conv
      "stride": 1,
      "l1_decay_mul": 0,
      "l2_decay_mul": 1,
      "pad": 2,
      "biases"
      "filters"

     */
    public class JSONParser
    {
        public static ConvolutionalNeuralNetwork Parse(string filename)
        {
            var jobject = JObject.Parse(File.ReadAllText(filename));
            var jsonLayers = jobject.GetValue("layers");
            var network = new ConvolutionalNeuralNetwork();
            var netLayers = new List<CNNLayer>();

            foreach (var jsonLayer in jsonLayers.ToArray())
            {
                var type = jsonLayer["layer_type"].ToObject<string>();
                CNNLayer layer;
                switch (type)
                {
                    case "input":
                        layer = new InputLayer();
                        break;

                    case "conv":
                        layer = new ConvolutionalLayer
                        {
                            FilterSize = jsonLayer["sx"].ToObject<int>(),
                            Stride = jsonLayer["stride"].ToObject<int>(),
                            L1Decay = jsonLayer["l1_decay_mul"].ToObject<double>(),
                            L2Decay = jsonLayer["l2_decay_mul"].ToObject<double>(),
                            Pad = jsonLayer["pad"].ToObject<int>(),
                            Biases  = Volume.ParseJSON(jsonLayer["biases"]),
                            Kernels = Volume.ArrayParseJSON(jsonLayer["filters"])
                        };
                        break;

                    case "relu":
                        layer = new ReLuLayer();
                        break;

                    case "pool":
                        layer = new SubsamplingLayer
                        {
                            FilterSize = jsonLayer["sx"].ToObject<int>(),
                            Stride = jsonLayer["stride"].ToObject<int>(),
                            Pad = jsonLayer["pad"].ToObject<int>(),
                        };

                        break;

                    case "fc":
                        layer = new FullyConnectedLayer
                        {
                            L1Decay = jsonLayer["l1_decay_mul"].ToObject<double>(),
                            L2Decay = jsonLayer["l2_decay_mul"].ToObject<double>(),
                            NeuronsCount = jsonLayer["out_depth"].ToObject<int>(),
                            Biases = Volume.ParseJSON(jsonLayer["biases"]),
                            Weights = Volume.ArrayParseJSON(jsonLayer["filters"])
                        };
                        break;

                    case "softmax":
                        layer = new SoftmaxLayer();
                        break;

                    default:
                        throw new ArgumentException("Unknown layer type");
                }

                if (netLayers.Count != 0)
                {
                    layer.InSize  = netLayers.Last().OutSize;
                    layer.InDepth = netLayers.Last().OutDepth;
                }

                layer.OutDepth = jsonLayer["out_depth"].ToObject<int>();
                layer.OutSize = new Size(
                    jsonLayer["out_sx"].ToObject<int>(),
                    jsonLayer["out_sy"].ToObject<int>());

                if (type == "fc")
                {
                    (layer as FullyConnectedLayer)._inputsCount =
                        layer.InSize.Width * layer.InSize.Height * layer.InDepth;
                }
                else if (type == "pool")
                {
                    (layer as SubsamplingLayer)._oldX = new int[layer.OutSize.Width * layer.OutSize.Height * layer.OutDepth];
                    (layer as SubsamplingLayer)._oldY = new int[layer.OutSize.Width * layer.OutSize.Height * layer.OutDepth];
                }
                else if (type == "softmax")
                {
                    (layer as SoftmaxLayer)._es = new double[layer.OutDepth];

                }

                layer.OutVolume = new Volume(layer.OutSize.Width, layer.OutSize.Height, layer.OutDepth, 0);
                netLayers.Add(layer);
            }

            network.Layers = netLayers;
            network.InputLayer = (InputLayer)netLayers.First();
            return network;
        }
    }
}