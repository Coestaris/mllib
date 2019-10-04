//
// Created by maxim on 10/4/19.
//

#include "main.h"

network_t* network;

task_t* xor_task(size_t index)
{
    task_t* task = createTask();
    int a = rand() % 2;
    int b = rand() % 2;
    int result = a ^ b;

    task->input[0] = (float)a;
    task->input[1] = (float)b;
    task->expected[0] = (float)result;
    return task;
}

void initNetwork()
{
    network = nw_build(INPUT_COUNT, OUTPUT_COUNT, HIDDEN, HIDDEN_COUNT);
    nw_fill(network, fm_randomAll, 1, 1);
}

void learn()
{
    nw_forwardPass(network, tasks[0]->input);
}

int main()
{
    srand(time(NULL));

    initNetwork();
    buildTasks(xor_task);

    learn();

    nw_print(network);
    nw_free(network);
    return 0;
}