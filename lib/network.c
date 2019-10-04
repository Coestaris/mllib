//
// Created by maxim on 10/4/19.
//

#include "network.h"

network_t* nw_build(size_t input, size_t output, size_t hidden, size_t hiddenCount)
{
    network_t* nw = malloc(sizeof(network_t));
    nw->inputNeuronsCount = input;
    nw->outputNeuronsCount = output;
    nw->hiddenLayersCount = hidden;
    nw->hiddenNeuronsCount = hiddenCount;

    nw->inputLayer = malloc(sizeof(neuron_t*) * input);
    nw->hiddenLayer = malloc(sizeof(neuron_t**) * hidden);
    nw->outputLayer = malloc(sizeof(neuron_t*) * output);

    for(size_t i = 0; i < input; i++) {

    }

    return nw;
}

void nw_fill(fillMode_t fillMode, float value, float bias)
{

}