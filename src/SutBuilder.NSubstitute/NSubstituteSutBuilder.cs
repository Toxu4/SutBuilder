using System;
using System.Threading;
using NSubstitute;

namespace SutBuilder.NSubstitute
{
    public class NSubstituteSutBuilder<T> : AbstractSutBuilder<T> where T : class 
    {
        public NSubstituteSutBuilder(Action<AbstractSutBuilder<T>> defaultConfig = null) : base(defaultConfig)
        {
        }
        
        protected override TStub CreateStub<TStub>()
        {
            return Substitute.For<TStub>();
        }

    }
}