using System;
using System.Collections.Generic;
using System.Linq;

namespace MLLib.AI.NEAT
{
    public class Genome
    {
        internal List<NodeGene> Nodes;
        internal Dictionary<int, ConnectionGene> Connections;

        public Genome(int inputs, int outputs, int hidden, InnovationCounter innovation)
        {
            Nodes = new List<NodeGene>();
            Connections = new Dictionary<int, ConnectionGene>();

            for(var i = 0; i < inputs; i++)
                Nodes.Add(new NodeGene(i, NodeGene.Type.Input));

            for(var i = 0; i < outputs; i++)
                Nodes.Add(new NodeGene(i, NodeGene.Type.Output));

            for(var i = 0; i < hidden; i++)
                Nodes.Add(new NodeGene(i, NodeGene.Type.Hidden));
        }

        internal void PushNode(NodeGene node)
        {
            Nodes.Add(node);
        }

        internal void PushConnection(ConnectionGene connection, InnovationCounter innovation)
        {
            Connections.Add(innovation.Get(), connection);
        }

        void MutationAddNode(Random random, InnovationCounter innovation)
        {
            var tries = 0;
            while (true)
            {
                var connection = Connections.Values.ToArray()[random.Next(Connections.Count)];
                if (!connection.enabled) if(tries++ > 100) return;

                var node = new NodeGene(Nodes.Count, NodeGene.Type.Hidden);
                Nodes.Add(node);

                connection.enabled = false;

                var inn1 = innovation.Get();
                Connections.Add(inn1, new ConnectionGene(node.ID, connection.outId, connection.weight, true, inn1));


                var inn2 = innovation.Get();
                Connections.Add(inn2, new ConnectionGene(connection.inId, node.ID, 1, true, inn2));
                break;
            }
        }
    }
}