using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ml.AI
{
    public struct TeacherMonitorInfo
    {
        public double TrainingCost;
        public double ValidationCost;
        public double TrainingAccuracy;
        public double ValidationAccuracy;
    }

    public class Teacher
    {
        public readonly List<TrainSample> TrainingTasks;
        public readonly List<TrainSample> TestTasks;
        public readonly List<TrainSample> ValidationTasks;

        public int BatchSize;
        public int TotalEpochCount;
        public int ErrorCalcFactor = 50;
        public Network Network;

        public bool MonitorTrainingCost = true;
        public bool MonitorValidationCost = false;
        public bool MonitorTrainingAccuracy = false;
        public bool MonitorValidationAccuracy = false;
        public bool SilentMode = false;

        public readonly List<TeacherMonitorInfo> MonitorInfos;
        public readonly Random Shuffler = new Random();

        public Action<int> BatchCallback;
        public Action<int> EpochCallback;

        public double Error => _errorSum / _n;
        public double TestDataAccuracy => CalculateAccuracy(TestTasks);
        public double TestDataCost => CalculateError(TestTasks);

        private readonly  TimeSpan _setupTime;
        private double _errorSum;
        private int _n;

        public Teacher(Network network,
            int totalEpochCount,
            List<TrainSample> trainSamples,
            List<TrainSample> testSamples = null,
            List<TrainSample> validataionSamples = null)
            : this(network, totalEpochCount, 0, trainSamples) {}

        public Teacher(Network network,
            int totalEpochCount, int batchSize,
            List<TrainSample> trainSamples,
            List<TrainSample> testSamples = null,
            List<TrainSample> validationSamples = null)
        {
            TotalEpochCount = totalEpochCount;
            BatchSize = batchSize;
            _errorSum = 0;
            _n = 0;

            TrainingTasks = trainSamples;
            TestTasks = testSamples;
            ValidationTasks = validationSamples;

            Network = network;
            MonitorInfos = new List<TeacherMonitorInfo>();
            if (BatchSize == 0) BatchSize = TrainingTasks.Count;
        }

        public void Teach()
        {
            for (var epoch = 0; epoch < TotalEpochCount; epoch++)
            {
                var index = 0;
                foreach (var batch in TrainingTasks.Shuffle(Shuffler).ToArray().Split(BatchSize))
                {
                    var batchArray = batch.ToArray();
                    foreach (var task in batchArray)
                    {
                        Network.ForwardPass(task.ToTrainData());
                        Network.BackProp(task.ToExpected());

                        if (index++ % ErrorCalcFactor == 0)
                        {
                            _errorSum += Network.CalculateError(task.ToExpected());
                            _n++;
                        }
                    }

                    Network.ApplyNudge(batchArray.Length, TrainingTasks.Count);
                    BatchCallback?.Invoke(index++);
                }

                var sb = new StringBuilder();
                var info = new TeacherMonitorInfo();
                if(!SilentMode) sb.AppendFormat("Epoch: {0}/{1}", epoch + 1, TotalEpochCount);
                if (MonitorTrainingCost)
                {
                    var error = Error;
                    if(!SilentMode) sb.AppendFormat(", Training cost: {0:F4}", error);
                    info.TrainingCost = epoch;
                }

                if (MonitorValidationCost && ValidationTasks != null)
                {
                    var error = CalculateError(ValidationTasks);
                    if(!SilentMode) sb.AppendFormat(", Validation cost: {0:F4}", error);
                    info.ValidationCost = epoch;
                }

                if (MonitorTrainingAccuracy)
                {
                    var accuracy = CalculateAccuracy(TrainingTasks);
                    if(!SilentMode) sb.AppendFormat(", Training Accuracy: {0:F4}%", accuracy * 100);
                    info.TrainingAccuracy = accuracy;
                }

                if (MonitorValidationAccuracy && ValidationTasks != null)
                {
                    var accuracy = CalculateAccuracy(ValidationTasks);
                    if(!SilentMode) sb.AppendFormat(", Validation Accuracy: {0:F4}%", accuracy * 100);
                    info.ValidationAccuracy = accuracy;
                }
                MonitorInfos.Add(info);
                if(!SilentMode) Console.WriteLine(sb);
                ResetError();
            }
        }

        public void Test(int count, bool shuffle)
        {
            if(TestTasks == null) return;

            var testDataCopy = TestTasks.Select(p => p).ToList();
            if (shuffle) testDataCopy = testDataCopy.Shuffle(Shuffler).ToList();

            for (int i = 0; i < count; i++)
            {
                var task = testDataCopy[i];
                var output = Network.ForwardPass(task.ToTrainData());

                var expected = "[" + string.Join(",", task.ToExpected().Select(p => p.ToString("F2"))) + "]";
                var got = "[" +  string.Join(",", output.Select(p => p.ToString("F2"))) + "]";

                if (task.CheckAssumption(output))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Suggestion is correct! ");

                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Expected and got: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(expected);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Suggestion is wrong! ");

                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("Expected ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(expected);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(" but got ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(got);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            }
        }

        public double CalculateAccuracy(List<TrainSample> samples)
        {
            var n = 0;
            foreach (var sample in samples)
            {
                if (sample.CheckAssumption(Network.ForwardPass(sample.ToTrainData())))
                    n++;
            }

            return n / (double) samples.Count;
        }

        public double CalculateError(IEnumerable<TrainSample> samples)
        {
            var n = 0;
            var sum = 0.0;
            foreach (var sample in samples)
            {
                Network.ForwardPass(sample.ToTrainData());
                sum += Network.CalculateError(sample.ToExpected());
                n++;
            }

            return sum / n;
        }

        public void ResetError()
        {
            _errorSum = 0;
            _n = 0;
        }
    }

    public static class Extension
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            if (size == array.Length)
            {
                yield return array;
                yield break;
            }

            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var elements = source.ToArray();
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                var swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }
    }
}