using System;

namespace ml.AIMath
{
    public class Matrix
    {
        public readonly int Rows;
        public readonly int Columns;

        private readonly double[,] _data;

        private Random _random;
        private GaussianRandom _gaussianRandom;

        public Matrix(double[] array, bool fillRows = true)
        {
            if (fillRows)
            {
                Rows = array.Length;
                Columns = 1;
                _data = new double[Rows, Columns];
                for (int i = 0; i < array.Length; i++)
                    _data[i, 0] = array[i];
            }
            else
            {
                Rows = 1;
                Columns = array.Length;
                _data = new double[Rows, Columns];
                for (int i = 0; i < array.Length; i++)
                    _data[0, i] = array[i];
            }

            _random = new Random();
            _gaussianRandom = new GaussianRandom(_random);
        }

        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _data = new double[rows, columns];

            _random = new Random();
            _gaussianRandom = new GaussianRandom(_random);
        }

        public Matrix(int rows, int columns, params double[] values)
        {
            Rows = rows;
            Columns = columns;
            _data = new double[rows, columns];

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    _data[r, c] = values[r * Columns + c];

            _random = new Random();
            _gaussianRandom = new GaussianRandom(_random);
        }

        public Matrix(Matrix matrix)
        {
            _data = (double[,])matrix._data.Clone();
            Columns = matrix.Columns;
            Rows = matrix.Rows;

            _random = new Random();
            _gaussianRandom = new GaussianRandom(_random);
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
                        Math.Abs(_data[r,c]),
                        _data[r, c] < 0 ? "-" : " ",
                        (r == Rows - 1 && c == Columns - 1) ? "]" : (c == Columns - 1 ? "],\n" : ", "));
                }
            }
            Console.WriteLine("]");
        }

        public void Fill(Func<int, int, double> fillFunc)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    _data[r, c] = fillFunc(r, c);
        }

        public void Fill(double x)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    _data[r, c] = x;
        }

        public void FillRandom(Random random = null)
        {
            if (random == null) random = _random;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    _data[r, c] = random.NextDouble();
                }
        }

        public void FillRandom(double min, double max, Random random = null)
        {
            if (min < max)
            {
                var a = min;
                min = max;
                max = a;
            }

            if (random == null) random = _random;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    _data[r, c] = Math.Abs(random.NextDouble()) * (max - min) + min;
                }
        }


        public void FillGaussianRandom(GaussianRandom random = null)
        {
            if (random == null) random = _gaussianRandom;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    _data[r, c] = random.Next();
                }
        }

        public void FillGaussianRandom(double mean, double dev, GaussianRandom random = null)
        {
            if (random == null) random = _gaussianRandom;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                {
                    _data[r, c] = random.Next(mean, dev);
                }
        }

        public void ApplyFunction(Func<double, double> func)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Columns; c++)
                    _data[r, c] = func(_data[r, c]);
        }

        public static Matrix operator+(Matrix a, Matrix b)
        {
            if(a.Rows != b.Rows && a.Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result._data[r, c] += b._data[r, c];
            return result;
        }

        public static Matrix operator+(Matrix a, double v)
        {
            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result._data[r, c] += v;

            return result;
        }

        public static Matrix operator-(Matrix a, Matrix b)
        {
            if(a.Rows != b.Rows && a.Columns != b.Columns)
                throw new ArgumentException("Matrices should have same size");

            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result._data[r, c] -= b._data[r, c];
            return result;
        }

        public static Matrix operator-(Matrix a, double v)
        {
            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result._data[r, c] -= v;

            return result;
        }

        public static Matrix operator*(Matrix a, Matrix b)
        {
            if(a.Columns != b.Rows)
                throw new ArgumentException("Matrices should have same size");

            var matrix = new Matrix(a.Rows, b.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    for (int k = 0; k < a.Columns; k++)
                        matrix._data[i, j] = matrix._data[i, j] + a._data[i, k] * b._data[k, j];
                }
            }
            return matrix;
        }

        public static Matrix operator*(Matrix a, double v)
        {
            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
                for (int c = 0; c < result.Columns; c++)
                    result._data[r, c] *= v;

            return result;
        }

        public static Matrix operator/(Matrix a, double v)
        {
            var result = new Matrix(a);
            for (int r = 0; r < result.Rows; r++)
               for (int c = 0; c < result.Columns; c++)
                   result._data[r, c] /= v;

            return result;
        }
    }
}