using System;
using System.Drawing;
using System.Threading;

namespace ml.AIMath
{
    public class Matrix
    {
        public readonly int Rows;
        public readonly int Columns;

        public Size Size => new Size(Columns, Rows);
        public double[,] Data;

        private static Random _random = new Random();
        private static GaussianRandom _gaussianRandom = new GaussianRandom(_random);

        public double this[int r, int c]
        {
            get { return Data[r, c]; }
            set { Data[r, c] = value; }
        }

        public Matrix(double[] array, int rows, int columns, bool flip = false)
        {
            if(array.Length != rows * columns)
                throw new ArgumentException();

            Data = new double[rows, columns];
            for (var r = 0; r < rows; r++)
                for (var c = 0; c < columns; c++)
                    if (flip)
                    {
                        Data[r, c] = array[c * rows + r];
                    }
                    else
                    {
                        Data[r, c] = array[r * columns + c];
                    }

            Rows = rows;
            Columns = columns;
        }

        public Matrix(double[] array, bool fillRows = true) : this(array, fillRows ? array.Length : 1, fillRows ? 1 : array.Length)
        { }

        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Data = new double[rows, columns];
        }

        public Matrix(int rows, int columns, params double[] values)
        {
            Rows = rows;
            Columns = columns;
            Data = new double[rows, columns];

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    Data[r, c] = values[r * Columns + c];
        }

        public Matrix(Matrix matrix)
        {
            Data = (double[,])matrix.Data.Clone();
            Columns = matrix.Columns;
            Rows = matrix.Rows;
        }

        public void Print()
        {
            Console.Write("[");
            for (int r = 0; r < Rows; r++)
            {
                if(r != 0)
                    Console.Write(" [");
                else
                    Console.Write("[");

                for (int c = 0; c < Columns; c++)
                {
                    Console.Write("{1}{0:F4}{2}",
                        Math.Abs(Data[r,c]),
                        Data[r, c] < 0 ? "-" : " ",
                        (r == Rows - 1 && c == Columns - 1) ? "]" : (c == Columns - 1 ? "],\n" : ", "));
                }
            }
            Console.WriteLine("]");
        }

        public Matrix Fill(Func<int, int, double> fillFunc)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    Data[r, c] = fillFunc(r, c);

            return this;
        }

        public Matrix Fill(double x)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                    Data[r, c] = x;

            return this;
        }

        public Matrix FillRandom(Random random = null)
        {
            if (random == null) random = _random;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] = random.NextDouble();

            return this;
        }

        public Matrix FillRandom(double min, double max, Random random = null)
        {
            if (min < max)
            {
                var a = min;
                min = max;
                max = a;
            }

            if (random == null) random = _random;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] = Math.Abs(random.NextDouble()) * (max - min) + min;

            return this;
        }


        public Matrix FillGaussianRandom(GaussianRandom random = null)
        {
            if (random == null) random = _gaussianRandom;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] = random.Next();

            return this;
        }

        public Matrix FillGaussianRandom(double mean, double dev, GaussianRandom random = null)
        {
            if (random == null) random = _gaussianRandom;
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] = random.Next(mean, dev);

            return this;
        }

        public Matrix ApplyFunction(Func<double, double> func)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] = func(Data[r, c]);

            return this;
        }

        public Matrix Add(Matrix b)
        {
            if(Rows != b.Rows && Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] += b.Data[r, c];

            return this;
        }

        public static Matrix operator+(Matrix a, Matrix b)
        {
            var matrix = new Matrix(a);
            return matrix.Add(b);
        }

        public Matrix Add(double v)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                    Data[r, c] += v;

            return this;
        }

        public static Matrix operator+(Matrix a, double v)
        {
            var matrix = new Matrix(a);
            return matrix.Add(v);
        }

        public Matrix Subtract(Matrix b)
        {
            if(Rows != b.Rows && Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] -= b.Data[r, c];

            return this;
        }

        public static Matrix operator-(Matrix a, Matrix b)
        {
            var result = new Matrix(a);
            return result.Subtract(b);
        }

        public Matrix Subtract(double v)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] -= v;

            return this;
        }

        public static Matrix operator-(Matrix a, double v)
        {
            var matrix = new Matrix(a);
            return matrix.Subtract(v);
        }

        public Matrix Multiply(Matrix b)
        {
            if(Rows != b.Rows && Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] *= b.Data[r, c];

            return this;
        }

        public static Matrix operator*(Matrix a, Matrix b)
        {
            var result = new Matrix(a);
            return result.Multiply(b);
        }

        public Matrix Multiply(double v)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] *= v;

            return this;
        }

        public static Matrix operator*(Matrix a, double v)
        {
            var matrix = new Matrix(a);
            return matrix.Multiply(v);
        }

        public Matrix Divide(double v)
        {
            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] /= v;

            return this;
        }

        public static Matrix operator/(Matrix a, double v)
        {
            var matrix = new Matrix(a);
            return matrix.Divide(v);
        }

        public Matrix Divide(Matrix b)
        {
            if(Rows != b.Rows && Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            for (var r = 0; r < Rows; r++)
            for (var c = 0; c < Columns; c++)
                Data[r, c] /= b.Data[r, c];

            return this;
        }

        public static Matrix operator/(Matrix a, Matrix b)
        {
            var matrix = new Matrix(a);
            return matrix.Divide(b);
        }


        public double[] ToArray(bool rows = true)
        {
            var result = new double[rows ? Rows : Columns];
            if (rows)
                for (var i = 0; i < Rows; i++)
                    result[i] = Data[i, 0];
            else
                for (var i = 0; i < Columns; i++)
                    result[i] = Data[0, i];

            return result;
        }

        public Matrix Transpose(Matrix destMatrix)
        {
            var w = Rows;
            var h = Columns;

            if(destMatrix.Rows != Columns ||
               destMatrix.Columns != Rows)
                throw new ArgumentException("Matrices should have same size");

            for (var i = 0; i < w; i++)
            for (var j = 0; j < h; j++)
                destMatrix.Data[j, i] = Data[i, j];

            return destMatrix;
        }

        public Matrix Transpose()
        {
            var w = Rows;
            var h = Columns;

            var result = new Matrix(h, w);

            for (var i = 0; i < w; i++)
                for (var j = 0; j < h; j++)
                    result.Data[j, i] = Data[i, j];

            return result;
        }

        public Matrix Dot(Matrix b)
        {
            var a = this;
            if(a.Columns != b.Rows)
                throw new ArgumentException("Matrices should have same size");

            var matrix = new Matrix(a.Rows, b.Columns);
            for (var i = 0; i < matrix.Rows; i++)
            for (var j = 0; j < matrix.Columns; j++)
            {
                for (var k = 0; k < a.Columns; k++)
                    matrix.Data[i, j] = matrix.Data[i, j] + a.Data[i, k] * b.Data[k, j];
            }

            return matrix;
        }

        public Matrix Dot(Matrix b, Matrix destMatrix)
        {
            var a = this;
            if(a.Columns != b.Rows ||
               a.Rows != destMatrix.Rows ||
               b.Columns != destMatrix.Columns)
                throw new ArgumentException("Matrices should have same size");

            for (var i = 0; i < destMatrix.Rows; i++)
            for (var j = 0; j < destMatrix.Columns; j++)
            {
                for (var k = 0; k < a.Columns; k++)
                    destMatrix.Data[i, j] = destMatrix.Data[i, j] + a.Data[i, k] * b.Data[k, j];
            }

            return destMatrix;
        }

        public double Sum()
        {
            double s = 0;

            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                s += Data[i, j];

            return s;
        }

        public Matrix Clip(double min, double max)
        {
            if (min > max)
            {
                var a = min;
                min = max;
                max = a;
            }

            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                if (Data[i, j] > max) Data[i, j] = max;
                else if (Data[i, j] < min) Data[i, j] = min;

            return this;
        }
    }
}