//
// Created by maxim on 10/4/19.
//

#ifndef ML_NETWORK_H
#define ML_NETWORK_H

#include <stddef.h>
#include <malloc.h>

#include "neuron.h"

typedef struct _network {
    size_t inputNeuronsCount;
    size_t outputNeuronsCount;
    size_t hiddenLayersCount;
    size_t hiddenNeuronsCount;

    neuron_t** inputLayer;
    neuron_t** outputLayer;
    neuron_t*** hiddenLayer;

} network_t;

typedef enum _fillMode {
    randomAll,
    randomValue,
    randomBias,
    manually,

} fillMode_t;

network_t* nw_build(size_t input, size_t output, size_t hidden, size_t hiddenCount);
void nw_fill(fillMode_t fillMode, float value, float bias);

#endif //ML_NETWORK_H
