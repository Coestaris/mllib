using System;

namespace ml.AIMath
{
	public class GaussianRandom
	{
		private bool _ready = false;
		private double _calculated = 0;

		private Random _random;

		public GaussianRandom(Random random)
		{
			_random = random;
		}

		public GaussianRandom(int seed)
		{
			_random = new Random(seed);
		}

		public GaussianRandom()
		{
			_random = new Random();
		}

		public double Next(double mean = 0, double dev = 1)
		{
			if (_ready)
			{
				_ready = false;
				return _calculated * dev + mean;
			}

			double u, v, s;
			do
			{
				u = 2.0 * _random.NextDouble() - 1.0;
				v = 2.0 * _random.NextDouble() - 1.0;
				s = u * u + v * v;
			}
			while (s > 1.0 || Math.Abs(s) < 0.0001);

			var r = Math.Sqrt(-2.0 * Math.Log(s) / s);
			_calculated = r * u;
			_ready = true;

			return r * v * dev + mean;
		}
	}
}