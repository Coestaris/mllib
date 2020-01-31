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
    }
}