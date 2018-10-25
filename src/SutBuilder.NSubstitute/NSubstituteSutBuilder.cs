using NSubstitute;

namespace SutBuilder.NSubstitute
{
    public class NSubstituteSutBuilder<T> : AbstractSutBuilder<T> where T : class 
    {
        protected override TStub CreateStub<TStub>()
        {
            return Substitute.For<TStub>();
        }
    }
}