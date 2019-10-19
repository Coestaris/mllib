using System;
using System.Collections.Generic;
using System.Linq;

namespace ml.AI
{
    public class TeacherTask
    {
        public readonly double[] Input;
        public readonly double[] Expected;

        public TeacherTask(double[] input, double[] expected)
        {
            Input = input;
            Expected = expected;
        }
    }

    public class Teacher
    {
        public int TasksDivision;
        public List<TeacherTask> Tasks;
        public Random Shuffler = new Random();
        public double SetupTime => _setupTime.TotalMilliseconds;

        private TimeSpan _setupTime;
        public Action<int> BatchCallback;

        private double _errorSum;
        private int _n;

        public Teacher(int tasksCount, List<ITrainSample> trainSamples)
            : this(tasksCount, tasksCount, trainSamples) {}

        public Teacher(int tasksCount, int tasksDivision, List<ITrainSample> trainSamples)
        {
            var start = DateTime.Now;
            TasksDivision = tasksDivision;
            Tasks = new List<TeacherTask>();
            for(var i = 0; i < tasksCount; i++)
                Tasks.Add(new TeacherTask(trainSamples[i].ToTrainData(), trainSamples[i].ToExpected()));

            _setupTime = TimeSpan.FromMilliseconds((DateTime.Now - start).TotalMilliseconds);
            _errorSum = 0;
            _n = 0;
        }

        public Teacher(int tasksCount, Func<int, TeacherTask> taskCreatorFunc)
            : this(tasksCount, tasksCount, taskCreatorFunc) {}

        public Teacher(int tasksCount, int tasksDivision, Func<int, TeacherTask> taskCreatorFunc)
        {
            var start = DateTime.Now;
            TasksDivision = tasksDivision;
            Tasks = new List<TeacherTask>();
            for(var i = 0; i < tasksCount; i++)
                Tasks.Add(taskCreatorFunc(i));

            _setupTime = TimeSpan.FromMilliseconds((DateTime.Now - start).TotalMilliseconds);
            _errorSum = 0;
            _n = 0;
        }

        public double Error => _errorSum / _n;

        public void Teach(INetwork network)
        {
            if (TasksDivision == Tasks.Count)
            {
                foreach (var task in Tasks.Shuffle(Shuffler))
                {
                    network.ForwardPass(task.Input);
                    network.BackProp(task.Expected);
                    network.ApplyNudge(1);

                    _errorSum += network.CalculateError(task.Expected);
                    _n++;
                }
            }
            else
            {
                var index = 0;
                foreach (var batch in Tasks.Shuffle(Shuffler).ToArray().Split(TasksDivision))
                {
                    var batchArray = batch.ToArray();
                    foreach (var task in batchArray)
                    {
                        network.ForwardPass(task.Input);
                        network.BackProp(task.Expected);

                        _errorSum += network.CalculateError(task.Expected);
                        _n++;
                    }

                    network.ApplyNudge(batchArray.Length);
                    BatchCallback?.Invoke(index++);
                }
            }
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
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static List<T> ShuffleList<T>(this Random r, IEnumerable<T> source)
        {
            var list = new List<T>();
            foreach (var item in source)
            {
                var i = r.Next(list.Count + 1);
                if (i == list.Count)
                {
                    list.Add(item);
                }
                else
                {
                    var temp = list[i];
                    list[i] = item;
                    list.Add(temp);
                }
            }
            return list;
        }
    }
}