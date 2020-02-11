namespace MLLib.AI.NEAT
{
    internal class ConnectionGene
    {
        public ConnectionGene(int inId, int outId, float weight, bool enabled, int innovation)
        {
            this.inId = inId;
            this.outId = outId;
            this.weight = weight;
            this.enabled = enabled;
            this.innovation = innovation;
        }

        internal int inId;
        internal int outId;
        internal float weight;
        internal bool enabled;
        internal int innovation;
    }
}