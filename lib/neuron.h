//
// Created by maxim on 10/4/19.
//

#ifndef ML_NEURON_H
#define ML_NEURON_H

#include <stddef.h>
#include <malloc.h>
#include <stdint.h>

struct _neuron;

typedef struct _axon {
    float weight;
    struct _neuron* a;
    struct _neuron* b;

} axon_t;

typedef struct _neuron {
    uint64_t id;

    float activation;
    float bias;

    axon_t** leftAxons;
    axon_t** rightAxons;
    size_t lAxonsCount;
    size_t rAxonsCount;

} neuron_t;

neuron_t* create_neuron(size_t lAxonsCount, size_t rAxonsCount);
void free_neuron(neuron_t* neuron);

axon_t* create_axon();
void free_axon(axon_t* axon);

#endif //ML_NEURON_H
