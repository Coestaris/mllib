using System.Collections.Generic;
using System.Linq;
using MLLib.AI.OBNN;

namespace MLLib.AI.GA
{
    public static class NeuroEvolution
    {
        public static Genome NNToGenome(NeuralNetwork neuralNetwork, ICreature creature)
        {
            var genes = new List<double>();
            foreach (var layer in neuralNetwork.Layers)
            {
                genes.AddRange(layer.Biases);
                genes.AddRange(layer.Weights);
            }
            return new Genome(genes, creature);
        }

        public static NeuralNetwork GenomeToNN(NeuralNetwork shape, Genome genome)
        {
            var newNN = (NeuralNetwork)shape.Clone();
            var offset = 0;
            foreach (var layer in newNN.Layers)
            {
                layer.Biases = genome.Genes.Skip(offset).Take(layer.Biases.Length).ToArray();
                offset += layer.Biases.Length;
                layer.Weights = genome.Genes.Skip(offset).Take(layer.Weights.Length).ToArray();
                offset += layer.Weights.Length;
            }

            return newNN;
        }
    }
}