using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using ml.AI;
using ml.AI.OBNN;

namespace HWDRecognizer
{
    internal class Program
    {
        private static void DoTest(OBNeuralNetwork network, Dataset set, int index)
        {
            HWImage image = set.DatasetImages[index];
            var input = image.ToTrainData();
            var output = network.ForwardPass(input);

            var counter = 0;
            var sortedOutput = output.Select(p => new double[] { p, counter++ }).OrderBy(p => p[0]).ToArray();

            var nearestSuggestions = string.Join(", ", sortedOutput.Take(3)
                .Select(p => $"{(int) p[1]} ({p[0]})"));

            if ((int) sortedOutput[0][1] == image.Number)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Suggestion is correct! ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Suggestion is wrong! ");
            }

            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Nearest suggestions: {0}", nearestSuggestions);
        }

        public static void Main(string[] args)
        {
            var dataset = new Dataset(
                    "data/dataset.data",
                    "data/datasetLabels.data",
                    "data/test.data",
                    "data/test.data"
                );

            Console.WriteLine("Loaded database ({1} images) in {0} ms.",
                dataset.LoadTime,
                dataset.DatasetImages.Count + dataset.TestImages.Count);

            var inputLayerSize = dataset.ImageSize.Width * dataset.ImageSize.Height;
            var network = new OBNeuralNetwork(new[] { inputLayerSize, 4, 4, 4, 4, 10 });

            network.Fill();
            network.LearningRate = 100;

            var teacher = new Teacher(dataset.DatasetImages.Count,
                dataset.DatasetImages.Cast<ITrainSample>().ToList());

            Console.WriteLine("Loaded network in {0} ms.", teacher.SetupTime);
            const int count = 10;
            for (int i = 0; i < count; i++)
            {
                teacher.Teach(network);
                Console.WriteLine("[{0}/{1}]. Error: {2}", i + 1, count, teacher.Error);
                teacher.ResetError();
            }

            var rng = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
            for (int i = 0; i < 15; i++)
                DoTest(network, dataset, rng.Next(0, dataset.DatasetImages.Count));
        }
    }
}