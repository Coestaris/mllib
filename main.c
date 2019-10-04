//
// Created by maxim on 10/4/19.
//

#include <stdio.h>
#include "lib/network.h"

int main()
{
    network_t* nw = nw_build(4, 2, 2, 3);

    nw_print(nw);
    nw_free(nw);
    return 0;
}