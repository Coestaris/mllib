//
// Created by maxim on 10/4/19.
//

#include "network.h"
#include "netmath.h"

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

    for(size_t i = 0; i < input; i++)
        nw->inputLayer[i] = create_neuron(0, hiddenCount);

    for(size_t i = 0; i < hidden; i++)
    {
        nw->hiddenLayer[i] = malloc(sizeof(neuron_t*) * hiddenCount);
        for(size_t j = 0; j < hiddenCount; j++)
            if(i == 0 && hidden == 1)
                nw->hiddenLayer[i][j] = create_neuron(input, output);
            else if(i == 0)
                nw->hiddenLayer[i][j] = create_neuron(input, hiddenCount);
            else if(i == hidden - 1)
                nw->hiddenLayer[i][j] = create_neuron(hiddenCount, output);
            else
                nw->hiddenLayer[i][j] = create_neuron(hiddenCount, hiddenCount);
    }

    for(size_t i = 0; i < output; i++)
        nw->outputLayer[i] = create_neuron(hiddenCount, 0);

    nwm_link(nw);
    return nw;
}

void nw_fill(network_t* network, fillMode_t fillMode, float value, float bias)
{
    for(size_t i = 0; i < network->inputNeuronsCount; i++)
        for(size_t j = 0; j < network->inputLayer[i]->rAxonsCount; j++)
        {
            network->inputLayer[i]->rightAxons[j]->weight = nwm_getWeight(fillMode, value, bias);
            network->inputLayer[i]->bias = nwm_getBias(fillMode, value, bias);
        }

    for(size_t layer = 0; layer < network->hiddenLayersCount; layer++)
        for(size_t i = 0; i < network->hiddenNeuronsCount; i++)
            for(size_t j = 0; j < network->hiddenLayer[layer][i]->rAxonsCount; j++)
            {
                network->hiddenLayer[layer][i]->rightAxons[j]->weight = nwm_getWeight(fillMode, value, bias);
                network->hiddenLayer[layer][i]->bias = nwm_getBias(fillMode, value, bias);
            }

    for(size_t i = 0; i < network->outputNeuronsCount; i++)
        network->outputLayer[i]->bias = nwm_getBias(fillMode, value, bias);
}

void nw_free(network_t* network)
{

}

void nw_print(network_t* network)
{
    for(size_t i = 0; i < network->inputNeuronsCount; i++)
    {
        printf("Input neuron #%li. ID: %lu, bias: %.3f, Points to [",
                i, network->inputLayer[i]->id,
               network->inputLayer[i]->bias);

        for(size_t j = 0; j < network->inputLayer[i]->rAxonsCount; j++)
            printf("%lu:%.3f%s",
                    network->inputLayer[i]->rightAxons[j]->b->id,
                    network->inputLayer[i]->rightAxons[j]->weight,
                    j != network->inputLayer[i]->rAxonsCount - 1 ? ", " : "]\n");
    }
    for(size_t layer = 0; layer < network->hiddenLayersCount; layer++)
    {
        for (size_t i = 0; i < network->hiddenNeuronsCount; i++)
        {
            printf("Hidden neuron #%li#%li. ID: %lu, bias: %.3f, Points to [",
                    layer, i, network->hiddenLayer[layer][i]->id,
                   network->hiddenLayer[layer][i]->bias);

            for (size_t j = 0; j < network->hiddenLayer[layer][i]->rAxonsCount; j++)
                printf("%lu:%.3f%s",
                        network->hiddenLayer[layer][i]->rightAxons[j]->b->id,
                        network->hiddenLayer[layer][i]->rightAxons[j]->weight,
                       j != network->hiddenLayer[layer][i]->rAxonsCount - 1 ? ", " : "]\n");

            /*for (size_t j = 0; j < network->hiddenLayer[layer][i]->lAxonsCount; j++)
                printf("%lu%s", network->hiddenLayer[layer][i]->leftAxons[j]->a->id,
                       j != network->hiddenLayer[layer][i]->lAxonsCount - 1 ? ", " : "]\n");*/
        }
    }
    for(size_t i = 0; i < network->outputNeuronsCount; i++)
    {
        printf("Output neuron #%li. ID: %lu, bias: %.3f\n",
                i, network->outputLayer[i]->id,
               network->inputLayer[i]->bias);

        /*for (size_t j = 0; j < network->outputLayer[i]->lAxonsCount; j++)
            printf("%lu%s", network->outputLayer[i]->leftAxons[j]->a->id,
                   j != network->outputLayer[i]->lAxonsCount - 1 ? ", " : "]\n");*/
    }
}

void nw_forwardPass(network_t* network, const float* inputData)
{
    for(size_t i = 0; i < network->inputNeuronsCount; i++)
        network->inputLayer[i]->activation = inputData[i];

    nmw_forwardPass(network->inputNeuronsCount, network->inputLayer);
}