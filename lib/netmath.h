//
// Created by maxim on 10/4/19.
//

#ifndef ML_NETMATH_H
#define ML_NETMATH_H

#include <math.h>
#include "network.h"

void nwm_link(network_t* network);
float nwm_getBias(fillMode_t fillMode, float value, float bias);
float nwm_getWeight(fillMode_t fillMode, float value, float bias);
void nmw_forwardPass(size_t neuronsCount, network_t* network);

#endif //ML_NETMATH_H
