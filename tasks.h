//
// Created by maxim on 10/4/19.
//

#include <stdint.h>
#include <stdlib.h>
#include <memory.h>

#ifndef ML_TASKS_H
#define ML_TASKS_H

#define INPUT_COUNT  2
#define OUTPUT_COUNT 1
#define HIDDEN       2
#define HIDDEN_COUNT 3

#define TASKS_COUNT  10000
#define TASKS_DIV    100
#define TASK_FUNC    xor_task

typedef struct _task {
    float* input;
    float* expected;

} task_t;

typedef task_t* (*taskGenFunction_t)(size_t index);

extern task_t** tasks;

void buildTasks(taskGenFunction_t function);
task_t* createTask();

#endif //ML_TASKS_H
