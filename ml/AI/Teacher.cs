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
            for (var i = 0; i < TasksDivision; i++)
            {
                network.ForwardPass(Tasks[i].Input);
                error += network.CalculateError(Tasks[i].Expected);
            }

            error /= TasksDivision;
            network.BackProp(error);
            Console.WriteLine(error);
        }
    }
}