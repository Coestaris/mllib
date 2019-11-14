namespace MLLib.AI.GA
{
    public interface ICreature
    {
        void Reset();
        object GetState();
        bool Step(int time);
        double GetFitness();

        ICreature CreatureChild();
        void Update(Genome genome);
    }
}