using System.Threading;
using NSubstitute;

namespace SutBuilder.NSubstitute
{
    public class NSubstituteSutBuilder<T> : AbstractSutBuilder<T> where T : class 
    {
        protected NSubstituteSutBuilder(params object[] initialStubs)
            :base(initialStubs)
        {
            
        }
        
        protected override TStub CreateStub<TStub>()
        {
            return Substitute.For<TStub>();
        }
    }
}