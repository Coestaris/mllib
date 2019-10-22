using System;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using ml.AI;
using ml.AI.OBNN;

namespace HWDRecognizer
{
    internal static class Program
    {
        private static int CountCorrect(INetwork network, Dataset set)
        {
            var count = 0;
            for(var i = 0; i < set.TestImages.Count; i++)
                if (DoTest(network, set, i, false))
                    count++;

            return count;
        }

        private static bool DoTest(INetwork network, Dataset set, int index, bool outputResult)
        {
            HWImage image = set.DatasetImages[index];
            var input = image.ToTrainData();
            var output = network.ForwardPass(input);

            var counter = 0;
            var sortedOutput = output.Select(p => new { value = p, index = counter++ }).OrderByDescending(p => p.value).ToArray();

            if (sortedOutput[0].index == image.Number)
            {
                if (outputResult)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Suggestion is correct! ");
                }
                else return true;
            }
            else
            {
                if (outputResult)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Suggestion is wrong! ");
                }
                else return false;
            }

            var nearestSuggestions = string.Join(", ", sortedOutput.Take(3)
                .Select(p => $"{p.index} ({p.value})"));

            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Nearest suggestions: {0}", nearestSuggestions);
            return false;
        }

        public static void Main(string[] args)
        {
            var dataset = new Dataset(
                    "data/dataset.data",
                    "data/datasetLabels.data",
                    "data/test.data",
                    "data/testLabels.data"
                );

            Console.WriteLine("Loaded database ({1} images) in {0} ms.",
                dataset.LoadTime,
                dataset.DatasetImages.Count + dataset.TestImages.Count);

            var inputLayerSize = dataset.ImageSize.Width * dataset.ImageSize.Height;
            var network = new ImprovedNeuralNetwork(new[] {inputLayerSize, 30, 10},
                new CrossEntropyCostFunction())
            {
                LearningRate = 0.5,
                RegularizationLambda = 5
            };

            network.FillGaussianRandom();

            var teacher = new Teacher(dataset.DatasetImages.Count, 30,
                dataset.DatasetImages.Cast<ITrainSample>().ToList());

            //network.Print();

            Console.WriteLine("Loaded network in {0} ms.", teacher.SetupTime);
            const int count = 10;
            for (int i = 0; i < count; i++)
            {
                teacher.Teach(network);
                Console.WriteLine("Epoch: {0}/{1}. Error: {2}. Recognized: {3}",
                    i + 1, count, teacher.Error, CountCorrect(network , dataset));

               // network.Print();

                teacher.ResetError();
            }

            var rng = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
            for (int i = 0; i < 15; i++)
                DoTest(network, dataset, rng.Next(0, dataset.DatasetImages.Count), true);
        }
    }
}