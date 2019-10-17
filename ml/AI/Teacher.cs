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

        private double _errorSum;
        private int _n;

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

        public void Teach(NeuralNetwork network, int count)
        {
            for (int i = 0; i < count; i++)
                TeachStep(network);
        }

        public void TeachStep(NeuralNetwork network)
        {
            foreach (var batch in Tasks.Shuffle(Shuffler).ToArray().Split(TasksDivision))
            {
                foreach (var task in batch)
                {
                    network.ForwardPass(task.Input);
                    network.BackProp(task.Expected);

                    _errorSum += network.CalculateError(task.Expected);
                    _n++;
                }

                //break;
                //network.ApplyNudge(batch.Count());
                //Tasks = Shuffler.ShuffleList(Tasks);
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