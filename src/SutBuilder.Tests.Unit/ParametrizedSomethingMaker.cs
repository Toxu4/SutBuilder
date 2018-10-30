namespace SutBuilder.Tests.Unit
{
    public class ParametrizedSomethingMaker
    {
        private readonly int _somethingToBeMade;

        public ParametrizedSomethingMaker(int somethingToBeMade)
        {
            _somethingToBeMade = somethingToBeMade;
        }
        public int MakeSomething()
        {
            return _somethingToBeMade;
        }
    }
}