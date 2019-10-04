cmake_minimum_required(VERSION 3.10)
project(ML)

add_executable(ML main.c lib/neuron.c lib/neuron.h lib/network.c lib/network.h lib/netmath.c lib/netmath.h main.h tasks.c tasks.h)

target_link_libraries(ML m)