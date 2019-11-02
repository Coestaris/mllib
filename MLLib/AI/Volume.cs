using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ml.AIMath;
using Newtonsoft.Json.Linq;

namespace ml.AI
{
    public struct Volume
    {
        private static Random _random = new Random();
        private static GaussianRandom _gaussianRandom = new GaussianRandom(_random);

        public int SX;
        public int SY;
        public int Depth;

        public double[] Weights;
        public double[] dWeights;

        public int WeightsRawLen => Weights.Length;
        public double[] WeightsRaw => Weights;
        public int dWeightsRawLen => dWeights.Length;
        public double[] dWeightsRaw => dWeights;

        public Volume(int length, double c = Double.NaN, bool gaussianRandom = true)
        {
            SX = 0;
            SY = 0;
            Depth = length;

            Weights  = new double[length];
            dWeights = new double[length];
            Fill(length,  c, gaussianRandom,
                double.IsNaN(c) && gaussianRandom ? Math.Sqrt(1.0 / length) : 0);
        }

        public Volume(int x, int y, int depth, double c = Double.NaN, bool gaussianRandom = true)
        {
            SX = x;
            SY = y;
            Depth = depth;

            var n = x * y * depth;
            Weights  = new double[n];
            dWeights = new double[n];
            Fill(n, c, gaussianRandom,
                double.IsNaN(c) && gaussianRandom ? Math.Sqrt(1.0 / n) : 0);
        }

        public Volume(Bitmap bitmap, bool rgb = false) : this(bitmap.Width, bitmap.Height, rgb ? 3 : 1, 0)
        {
            if (!rgb)
            {
                for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                    Set(x, y, 0, NormalizeColor(bitmap.GetPixel(x, y)));
            }
            else
            {
                for(var d = 0; d < 3; d++)
                for (var x = 0; x < bitmap.Width; x++)
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var c = bitmap.GetPixel(x, y);
                    var value = 0;
                    if (d == 0) value = c.R;
                    else if (d == 1) value = c.G;
                    else value = c.B;
                    Set(x, y, d, value / 256.0);
                }
            }
        }

        public static double NormalizeColor(Color color)
        {
            return (color.R + color.B + color.G) / (3.0 * 256);
        }

        private void Fill(int length, double c, bool gaussianRandom, double scale)
        {
            if (double.IsNaN(c))
            {
                if(gaussianRandom)
                    for (int i = 0; i < length; i++)
                        Weights[i] = _gaussianRandom.Next();
                else
                    for (int i = 0; i < length; i++)
                        Weights[i] = _random.Next();
            }
            else
            {
                for (int i = 0; i < length; i++)
                    Weights[i] = c;
            }
        }

        public double Get(int x, int y, int depth)
        {
            return Weights[(SX * y + x) * Depth + depth];
        }

        public void Set(int x, int y, int depth, double value)
        {
            Weights[(SX * y + x) * Depth + depth] = value;
        }

        public void Add(int x, int y, int depth, double value)
        {
            Weights[(SX * y + x) * Depth + depth] += value;
        }

        public double GetGrad(int x, int y, int depth)
        {
            return dWeights[(SX * y + x) * Depth + depth];
        }

        public void SetGrad(int x, int y, int depth, double value)
        {
            dWeights[(SX * y + x) * Depth + depth] = value;
        }

        public void AddGrad(int x, int y, int depth, double value)
        {
            dWeights[(SX * y + x) * Depth + depth] += value;
        }

        public void AddVolume(Volume v, double scale = 1)
        {
            for (var i = 0; i < v.Weights.Length; i++)
                Weights[i] += v.Weights[i] * scale;
        }

        public void SetDConstant(double c)
        {
            for (var i = 0; i < dWeights.Length; i++)
                dWeights[i] = c;
        }

        public void SetConstant(double c)
        {
            for (var i = 0; i < Weights.Length; i++)
                Weights[i] = c;
        }

        public void Print(int depth)
        {
            for (var y = 0; y < SY; y++)
            {
                for (var x = 0; x < SX; x++)
                {
                    Console.Write("{0:F3}{1}", Get(x, y, depth), x == SX - 1 ? "\n" : ", ");
                }
            }
        }

        public Volume Clone()
        {
            var v = new Volume
            {
                SX = SX,
                SY = SY,
                Depth = Depth,
                Weights = new double[SX * SY * Depth],
                dWeights = new double[SX * SY * Depth]
            };

            Weights.CopyTo(v.Weights, 0);
            dWeights.CopyTo(v.dWeights, 0);

            return v;
        }

        public static Volume ParseJSON(JToken jToken)
        {
            var v = new Volume
            {
                SX = jToken["sx"].ToObject<int>(),
                SY = jToken["sy"].ToObject<int>(),
                Depth = jToken["depth"].ToObject<int>()
            };
            v.Weights = new double[v.SX * v.SX * v.Depth];
            v.dWeights = new double[v.SX * v.SX * v.Depth];
            var array = jToken["w"];
            var i = 0;
            foreach (var child in array.Children())
            {
                v.Weights[i++] = child.ToObject<double>();
            }
            return v;
        }

        public static Volume[] ArrayParseJSON(JToken jToken)
        {
            return jToken.ToArray().Select(ParseJSON).ToArray();
        }
    }
}