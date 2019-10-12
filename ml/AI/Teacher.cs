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

        public Teacher(int tasksCount, int tasksDivision, Func<int, TeacherTask> taskCreatorFunc)
        {
            TasksDivision = tasksDivision;
            Tasks = new List<TeacherTask>();
            for(var i = 0; i < tasksCount; i++)
                Tasks.Add(taskCreatorFunc(i));
        }

        public void Teach(NeuralNetwork network, int count)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (var batch in Tasks.ToArray().Split(TasksDivision))
                {
                    foreach (var task in batch)
                    {
                        network.ForwardPass(task.Input);
                        network.BackProp(task.Expected);
                    }

                    //network.ApplyNudge(batch.Count());
                    Tasks = Shuffler.NextList(Tasks);
                }
            }
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

        public static List<T> NextList<T>(this Random r, IEnumerable<T> source)
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