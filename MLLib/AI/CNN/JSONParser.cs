using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ml.AI.CNN.Layers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ml.AI.CNN
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
                        layer = new InputLayer(); break;
                    case "conv":
                        layer = new ConvolutionalLayer
                        {
                            Stride = jsonLayer["stride"].ToObject<int>(),
                            L1Decay = jsonLayer["l1_decay_mul"].ToObject<double>(),
                            L2Decay = jsonLayer["l2_decay_mul"].ToObject<double>(),
                            Pad = jsonLayer["pad"].ToObject<int>(),
                            Biases  = Volume.ParseJSON(jsonLayer["biases"]),
                            Kernels = Volume.ArrayParseJSON(jsonLayer["filters"])
                        };
                        break;

                    case "relu": layer = new ReLuLayer(); break;
                    case "pool": layer = new SubsamplingLayer(); break;
                    case "fc": layer = new FullyConnectedLayer(); break;
                    case "softmax": layer = new SoftmaxLayer(); break;
                    default:
                        throw new ArgumentException("Unknown layer type");
                }

                if (type != "input")
                {
                    layer.InDepth = jsonLayer["in_depth"].ToObject<int>();
                    layer.InSize =  new Size(
                        jsonLayer["sx"].ToObject<int>(),
                        jsonLayer["sy"].ToObject<int>());
                }

                layer.OutDepth = (int)jsonLayer["out_depth"].ToObject<int>();
                layer.OutSize = new Size(
                    jsonLayer["out_sx"].ToObject<int>(),
                    jsonLayer["out_sy"].ToObject<int>());
            }

            return null;
        }
    }
}