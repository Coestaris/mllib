using System;
using System.Collections.Generic;
using System.Linq;
using ml.AI;
using ml.AI.OBNN;

namespace TennisClassifier
{
    internal class Program
    {
        private static Random _random = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
        private static List<WeatherCondition> _weatherConditions;
        private const int DataCount = 500000;
        private const int TestsCount = 100;

        private static void DoTest(OBNeuralNetwork network)
        {
            int wrong = 0;
            for (int i = DataCount; i < DataCount + TestsCount; i++)
            {
                var input = _weatherConditions[i].ToTrainData();
                var output = network.ForwardPass(input);
                var result = _weatherConditions[i].ShouldPlay();

                if (output[0] > .9 && result ||
                    output[0] < .1 && !result)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Suggestion is correct! ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Suggestion is wrong! ");
                    wrong++;
                }

                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Diff: {0:F4}", output[0] - (result ? 1 : 0));
            }

            Console.WriteLine("{0} of {1} ({2:F4}%) were wrong!", wrong, TestsCount, wrong / (double)TestsCount * 100);
        }

        public static void Main(string[] args)
        {
            _weatherConditions = new List<WeatherCondition>();
            for (int i = 0; i < DataCount + TestsCount; i++)
            {
                _weatherConditions.Add(new WeatherCondition(
                    _random.NextDouble(),
                    _random.NextDouble(),
                    _random.NextDouble(),
                    _random.NextDouble()));

                /*
                var outlook = Outlook.Overcast;
                var humidity = Humidity.High;
                var temperature = Temperature.Cool;

                switch (_random.Next(0, 2))
                {
                    case 0: outlook = Outlook.Sunny; break;
                    case 1: outlook = Outlook.Overcast; break;
                    case 2: outlook = Outlook.Rain; break;
                }
                switch (_random.Next(0, 2))
                {
                    case 0: humidity = Humidity.High; break;
                    case 1: humidity = Humidity.Normal; break;
                }
                var windy = _random.Next() % 2 == 0;
                switch (_random.Next(0, 3))
                {
                    case 0: temperature = Temperature.Hot; break;
                    case 1: temperature = Temperature.Mild; break;
                    case 2: temperature = Temperature.Cool; break;
                }

                _weatherConditions.Add(new WeatherCondition(
                    outlook, humidity, windy, temperature));*/
            }

            var network = new OBNeuralNetwork(new[] { 4, 16, 16, 1 });
            network.LearningRate = 1;

            network.Fill();

            var teacher = new Teacher(DataCount, _weatherConditions.Cast<ITrainSample>().ToList());

            Console.WriteLine("Loaded network in {0} ms.", teacher.SetupTime);

            for (int i = 0; i < 20; i++)
            {
                teacher.Teach(network);
                Console.WriteLine("{0}. Error: {1}", i, teacher.Error);
            }

            teacher.ResetError();

            DoTest(network);
        }
    }
}