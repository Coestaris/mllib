//
// Created by maxim on 10/4/19.
//

#include "neuron.h"
uint64_t globalNeuronCounter = 0;

neuron_t* create_neuron(size_t lAxonsCount, size_t rAxonsCount)
{
    neuron_t* neuron = malloc(sizeof(neuron_t));
    neuron->lAxonsCount = lAxonsCount;
    neuron->rAxonsCount = rAxonsCount;

    if(lAxonsCount == 0) neuron->leftAxons = NULL;
    else neuron->leftAxons = malloc(sizeof(axon_t*) * lAxonsCount);

    if(rAxonsCount == 0) neuron->rightAxons = NULL;
    else neuron->rightAxons = malloc(sizeof(axon_t*) * rAxonsCount);
    neuron->id = globalNeuronCounter++;

    return neuron;
}

void free_neuron(neuron_t* neuron)
{
    free(neuron->leftAxons);
    free(neuron->rightAxons);
    free(neuron);
}

axon_t* create_axon()
{
    axon_t* axon = malloc(sizeof(axon_t));
    return axon;
}

void free_axon(axon_t* axon)
{
    free(axon);
}
