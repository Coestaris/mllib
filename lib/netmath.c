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