//
// Created by maxim on 10/4/19.
//

#include "neuron.h"

neuron_t* create_neuron()
{
    neuron_t* neuron = malloc(sizeof(neuron_t));
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
