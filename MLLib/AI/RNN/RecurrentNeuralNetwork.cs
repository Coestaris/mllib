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

        private 

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
        }

        public Matrix[] Pass(int[] inputIndexes, int[] targetIndexes, Matrix hiddenState)
        {
            //Forward Pass
            var xs = new Matrix[inputIndexes.Length];
            var hs = new Matrix[inputIndexes.Length + 1];
            var ys = new Matrix[inputIndexes.Length];
            var ps = new Matrix[inputIndexes.Length];
            hs[0] = new Matrix(hiddenState);
            var loss = 0.0;

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

        public void Train()
        {
            var mhBias = new Matrix(hBias.Rows, hBias.Columns);
            var myBias = new Matrix(yBias.Rows, yBias.Columns);
            var mxhW = new Matrix(xhW.Rows, xhW.Columns);
            var mhhW = new Matrix(hhW.Rows, hhW.Columns);
            var mhyW = new Matrix(hyW.Rows, hyW.Columns);
            var p = 0;
            var n = 0;

            Matrix hprev = null;

            while (true)
            {
                if (p + OutputCount + 1 >= Data.Length || n == 0)
                {
                    hprev = new Matrix(HiddenCount, 1);
                    p = 0;
                }


            }
        }
    }
}