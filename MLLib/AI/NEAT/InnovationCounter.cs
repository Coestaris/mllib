namespace MLLib.AI.NEAT
{
    public class InnovationCounter
    {
        private int _innovation;

        public int Get()
        {
            return _innovation++;
        }

        public void Reset()
        {
            _innovation = 0;
        }
    }
}