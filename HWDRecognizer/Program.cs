using System.Linq;

namespace HWDRecognizer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var dataset = new Dataset(
                    "data/dataset.data",
                    "data/datasetLabels.data",
                    "data/test.data",
                    "data/test.data"
                );

            var i = 0;
            dataset.DatasetImages.Take(10).ToList().ForEach(p => p.ToBitmap().Save(i++ + ".png"));
        }
    }
}