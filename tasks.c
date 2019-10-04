//
// Created by maxim on 10/4/19.
//

#include "tasks.h"

task_t** tasks;

void buildTasks(taskGenFunction_t function)
{
    tasks = malloc(sizeof(task_t*) * TASKS_COUNT);
    for(size_t i = 0; i < TASKS_COUNT; i++)
        tasks[i] = function(i);
}

task_t* createTask()
{
    task_t* task = malloc(sizeof(task));
    task->input = malloc(sizeof(float) * INPUT_COUNT);
    memset(task->input, 0, sizeof(float) * INPUT_COUNT);

    task->expected = malloc(sizeof(float) * OUTPUT_COUNT);
    memset(task->expected, 0, sizeof(float) * OUTPUT_COUNT);
    return task;
}