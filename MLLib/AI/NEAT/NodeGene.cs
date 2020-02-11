using System.Collections.Generic;

namespace MLLib.AI.NEAT
{

    internal class NodeGene
    {
        internal enum Type
        {
            Input,
            Output,
            Hidden
        }

        internal int ID;
        internal Type NodeType;

        internal List<ConnectionGene> InputConnections;

        public NodeGene(int id, Type nodeType)
        {
            ID = id;
            NodeType = nodeType;
        }
    }
}