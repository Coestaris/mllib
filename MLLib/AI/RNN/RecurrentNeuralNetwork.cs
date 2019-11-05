using System;
using System.Collections.Generic;
using System.Linq;
using ml.AIMath;

namespace ml.AI.RNN
{
    //Single layer char based RNN
    public class RecurrentNeuralNetwork
    {
        public int HiddenCount;
        public int OutputCount;
        public double LR;

        //x - input
        //h - hidden layer
        //y - output
        private Matrix hBias;
        private Matrix yBias;
        private Matrix xhW;
        private Matrix hhW;
        private Matrix hyW;

        public char[] Data;
        public char[] Vocab;

        private Dictionary<int, char> IndexToChar;
        private Dictionary<char, int> CharToIndex;
        private Matrix hprev;

        public RecurrentNeuralNetwork(string input, int hiddenCount, int outputCount, double lr)
        {
            Data = input.ToCharArray();
            Vocab = Data.Distinct().ToArray();

            HiddenCount = hiddenCount;
            OutputCount = outputCount;
            LR = lr;

            xhW = new Matrix(HiddenCount, Vocab.Length).FillGaussianRandom().Multiply(0.01);
            hhW = new Matrix(HiddenCount, HiddenCount).FillGaussianRandom().Multiply(0.01);
            hyW = new Matrix(Vocab.Length, HiddenCount).FillGaussianRandom().Multiply(0.01);

            hBias = new Matrix(HiddenCount, 1).Fill(0);
            yBias = new Matrix(Vocab.Length, 1).Fill(0);

            IndexToChar = new Dictionary<int, char>();
            CharToIndex = new Dictionary<char, int>();
            for (var i = 0; i < Vocab.Length; i++)
            {
                IndexToChar.Add(i, Vocab[i]);
                CharToIndex.Add(Vocab[i], i);
            }
        }

        private Matrix[] Pass(int[] inputIndexes, int[] targetIndexes, Matrix hiddenState, out double loss)
        {
            //Forward Pass
            var xs = new Matrix[inputIndexes.Length];
            var hs = new Matrix[inputIndexes.Length + 1];
            var ys = new Matrix[inputIndexes.Length];
            var ps = new Matrix[inputIndexes.Length];
            hs[0] = new Matrix(hiddenState);
            loss = 0.0;

            //calculating hidden states
            for (var t = 0; t < inputIndexes.Length; t++)
            {
                //input labels
                xs[t] = new Matrix(Vocab.Length, 1);
                xs[t][inputIndexes[t], 0] = 1;

                //current hidden state
                hs[t + 1] = xhW.Dot(xs[t]).Add(hhW.Dot(hs[t])).Add(hBias).ApplyFunction(ActivationFunctions.TanH);

                //unnormalized output
                ys[t] = hyW.Dot(hs[t + 1]).Add(yBias);

                //normalized output
                var exp = new Matrix(ys[t]).ApplyFunction(Math.Exp);
                ps[t] = exp.Divide(exp.Sum());

                //function loss
                loss += -Math.Log(ps[t][targetIndexes[t], 0]);
            }

            //Backward Pass
            var dhBias = new Matrix(hBias.Rows, hBias.Columns);
            var dyBias = new Matrix(yBias.Rows, yBias.Columns);
            var dxhW   = new Matrix(xhW.Rows, xhW.Columns);
            var dhhW   = new Matrix(hhW.Rows, hhW.Columns);
            var dhyW   = new Matrix(hyW.Rows, hyW.Columns);

            var dhnext = new Matrix(hs[0]).Fill(0);
            //calculating gradients
            for (var t = inputIndexes.Length - 1; t >= 0; t--)
            {
                //input derivative
                var dy = new Matrix(ps[t]);
                dy[targetIndexes[t], 0] -= 1;

                dhyW.Add(dy.Dot(hs[t + 1].Transpose()));
                dyBias.Add(dy);

                //hidden rerivative
                var dh = hyW.Transpose().Dot(dy).Add(dhnext);
                var dhraw = new Matrix(hs[t + 1]).Multiply(hs[t + 1]).Subtract(1).Multiply(-1).Multiply(dh);

                dhBias.Add(dhraw);

                dxhW.Add(dhraw.Dot(xs[t].Transpose()));
                dhhW.Add(dhraw.Dot(hs[t].Transpose()));
                dhnext = hhW.Transpose().Dot(dhraw);
            }

            //clipping values
            dhBias.Clip(-5, 5);
            dyBias.Clip(-5, 5);
            dxhW.Clip(-5, 5);
            dhhW.Clip(-5, 5);
            dhyW.Clip(-5, 5);

            return new[]
            {
                dhBias,
                dyBias,
                dxhW,
                dhhW,
                dhyW
            };
        }

        public double[] GetNextCharProbability(char ch, out char[] outCh)
        {
            var x = new Matrix(Vocab.Length, 1);
            x[CharToIndex[ch], 0] = 1;
            var h = hprev;

            h = xhW.Dot(x).Add(hhW.Dot(h)).Add(hBias).ApplyFunction(ActivationFunctions.TanH);
            var y = hyW.Dot(h).Add(yBias);

            //normalized output
            var exp = new Matrix(y).ApplyFunction(Math.Exp);
            var p = exp.Divide(exp.Sum()).ToArray();

            var index = 0;
            var ordered = p.Select(prob => new {i = index++, v = prob}).OrderByDescending(prob => prob.v).ToArray();

            outCh = ordered.Select(v => IndexToChar[v.i]).ToArray();
            return ordered.Select(v => v.v).ToArray();
        }

        public string Sample(char start, int count)
        {
            var result = start.ToString();
            var x = new Matrix(Vocab.Length, 1);
            x[CharToIndex[start], 0] = 1;
            Matrix h = hprev;

            for (int i = 0; i < count; i++)
            {
                h = xhW.Dot(x).Add(hhW.Dot(h)).Add(hBias).ApplyFunction(ActivationFunctions.TanH);
                var y = hyW.Dot(h).Add(yBias);

                //normalized output
                var exp = new Matrix(y).ApplyFunction(Math.Exp);
                var p = exp.Divide(exp.Sum()).ToArray();

                var maxP = double.MinValue;
                var maxPIndex = -1;
                for(var j = 0; j < p.Length; j++)
                    if (p[j] > maxP)
                    {
                        maxP = p[j];
                        maxPIndex = j;
                    }

                result += IndexToChar[maxPIndex];
                x.Fill(0)[maxPIndex, 0] = 1;
            }

            return result;
        }

        public void Train(int epoch, int callbackActivation, Func<int, double, bool> callback)
        {
            const double softingLoss = 0.9;
            const double learningSmophing = 1e-8;

            var mhBias = new Matrix(hBias.Rows, hBias.Columns);
            var myBias = new Matrix(yBias.Rows, yBias.Columns);
            var mxhW = new Matrix(xhW.Rows, xhW.Columns);
            var mhhW = new Matrix(hhW.Rows, hhW.Columns);
            var mhyW = new Matrix(hyW.Rows, hyW.Columns);
            var p = 0;
            var n = 0;
            var softLoss = 0.0;

            hprev = null;
            var parameters = new[] { hBias, yBias, xhW, hhW, hyW, };
            var mparameters = new[] {mhBias, myBias, mxhW, mhhW, mhyW};

            while (n < epoch)
            {
                if (p + OutputCount + 1 >= Data.Length || n == 0)
                {
                    hprev = new Matrix(HiddenCount, 1);
                    p = 0;
                }

                var inputs = Data
                    .Skip(p)
                    .Take(OutputCount)
                    .Select(ch => CharToIndex[ch])
                    .ToArray();

                var targets = Data
                    .Skip(p + 1)
                    .Take(OutputCount)
                    .Select(ch => CharToIndex[ch])
                    .ToArray();

                if (n % callbackActivation == 0 && n != 0)
                    if(callback(n, softLoss)) return;


                var loss = 0.0;
                var derivatives = Pass(inputs, targets, hprev, out loss);
                softLoss = softingLoss * softLoss + (1 - softingLoss) * loss;

                for (int i = 0; i < parameters.Length; i++)
                {
                    mparameters[i].Add(derivatives[i] * derivatives[i]);
                    parameters[i] .Add(
                        (derivatives[i] / new Matrix(mparameters[i]).Add(learningSmophing).ApplyFunction(Math.Sqrt)).Multiply(-LR));
                }

                n++;
                p++;
            }
        }
    }
}