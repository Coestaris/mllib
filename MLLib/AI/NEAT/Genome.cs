using System.Collections.Generic;

namespace MLLib.AI.NEAT
{
    public class Genome
    {
        internal Dictionary<int, NodeGene> Genes;
        internal Dictionary<int, ConnectionGene> Connections;

        public Genome(int inputs, int outputs)
        {
            Genes = new Dictionary<int, NodeGene>();
            Connections = new Dictionary<int, ConnectionGene>();
        }
    }
}