using System;
using System.Collections.Generic;

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

        public Teacher(int tasksCount, int tasksDivision, Func<int, TeacherTask> taskCreatorFunc)
        {
            TasksDivision = tasksDivision;
            Tasks = new List<TeacherTask>();
            for(var i = 0; i < tasksCount; i++)
                Tasks.Add(taskCreatorFunc(i));
        }

        public void Teach(NeuralNetwork network)
        {
            var error = 0.0;
            for (var i = 0; i < 1; i++)
            {
                network.ForwardPass(new double[2] { 0.05, 0.10 });
                error += network.CalculateError(new double[2] { 0.01, 0.99 });
            }

            //error /= TasksDivision;
            network.BackProp(new double[2] { 0.01, 0.99 }, error);
            Console.WriteLine(error);
        }
    }
}