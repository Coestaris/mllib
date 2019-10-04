//
// Created by maxim on 10/4/19.
//

#include "netmath.h"

void nwm_link(network_t* network)
{
    for(size_t i = 0; i < network->inputNeuronsCount; i++)
    {
        for (size_t j = 0; j < network->hiddenNeuronsCount; j++)
        {
            axon_t* axon = create_axon();
            axon->a = network->inputLayer[i];
            axon->b = network->hiddenLayer[0][j];

            network->inputLayer[i]->rightAxons[j] = axon;
            network->hiddenLayer[0][j]->leftAxons[i] = axon;
        }
    }

    for(size_t i = 0; i < network->outputNeuronsCount; i++)
    {
        for (size_t j = 0; j < network->hiddenNeuronsCount; j++)
        {
            axon_t* axon = create_axon();
            axon->a = network->hiddenLayer[0][j];
            axon->b = network->outputLayer[i];
            network->outputLayer[i]->leftAxons[j] = axon;
            network->hiddenLayer[network->hiddenLayersCount - 1][j]->rightAxons[i] = axon;
        }
    }

    for(size_t layer = 1; layer < network->hiddenLayersCount; layer++)
    {
        for(size_t i = 0; i < network->hiddenNeuronsCount; i++)
            for (size_t j = 0; j < network->hiddenNeuronsCount; j++)
            {
                axon_t* axon = create_axon();
                axon->a = network->hiddenLayer[layer - 1][i];
                axon->b = network->hiddenLayer[layer][j];

                network->hiddenLayer[layer - 1][i]->rightAxons[j] = axon;
                network->hiddenLayer[layer][j]->leftAxons[i] = axon;
            }
    }
}
#define randFloat (float)((float)rand() / RAND_MAX);

float nwm_getBias(fillMode_t fillMode, float value, float bias)
{
    switch(fillMode)
    {
        case fm_randomAll:
            return randFloat;
        case fm_manually:
            return bias;
        case fm_randomBias:
            return randFloat;
        case fm_randomWeight:
            return bias;
    }
}

float nwm_getWeight(fillMode_t fillMode, float weight, float bias)
{
    switch(fillMode)
    {
        case fm_randomAll:
            return randFloat;
        case fm_manually:
            return weight;
        case fm_randomBias:
            return weight;
        case fm_randomWeight:
            return randFloat;
    }
}

float sigmoid(float a)
{
    return 1.0f / (1.0f+ powf(M_E, -a));
}

float hyperbolic(float a)
{
    float etoa = powf(M_E, a);
    float etoma = powf(M_E, -a);
    return (etoa - etoma) / (etoa + etoma);
}

float reLu(float a)
{
    return fmaxf(0, a);
}

#define activationFunc sigmoid

void nmw_forwardPass(size_t neuronsCount, network_t* network)
{
    neuron_t** layer = network->inputLayer;
    size_t layerSize = network->inputNeuronsCount;

    neuron_t** nextLayer = network->hiddenLayer[0];
    size_t nextLayerSize = network->hiddenNeuronsCount;
    size_t index = 0;

    while(index <= network->hiddenLayersCount + 1)
    {
        for(size_t n = 0; n < nextLayerSize; n++)
        {
            float sum = nextLayer[n]->bias;
            for (size_t c = 0; c < layerSize; c++)
                sum += layer[c]->rightAxons[n]->weight * layer[c]->activation;
            nextLayer[n]->activation = activationFunc(sum);
        }

        layer = nextLayer;
        if(index >= network->hiddenLayersCount)
            nextLayer = network->outputLayer;
        else
            nextLayer = network->hiddenLayer[index + 1];

        index++;
    }
}
