using System;
using NUnit.Framework;
using SutBuilder.NSubstitute;

namespace SutBuilder.Tests.Unit
{
    [TestFixture]
    public class NSubstituteTestFixtureBase<TSut> where TSut : class
    {       
        protected readonly NSubstituteSutBuilder<TSut> SutBuilder;

        protected NSubstituteTestFixtureBase(Action<AbstractSutBuilder<TSut>> defaultConfig = null)
        {
            SutBuilder = new NSubstituteSutBuilder<TSut>(defaultConfig);
        }

        [SetUp]
        public virtual void SetUp()
        {
            SutBuilder.Reset();
        }
    }
}