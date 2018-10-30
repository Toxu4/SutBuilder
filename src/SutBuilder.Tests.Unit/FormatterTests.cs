using NSubstitute;
using NUnit.Framework;
using SutBuilder.NSubstitute;

namespace SutBuilder.Tests.Unit
{
    [TestFixture]
    public class FormatterTests
    {
        private class SutBuilder : NSubstituteSutBuilder<Formatter>
        {
            public SutBuilder(params object[] initialStubs)
            : base(initialStubs.Length > 0 
                ? initialStubs 
                : new object[] { new SomethingMaker(), new ParametrizedSomethingMaker(256) })
            {
                Configure<IFormatProvider>(fp => fp.GetFormat().Returns("Hello {0}!"));
                Configure<IArgumentsProvider>(ap => ap.GetArguments().Returns(new object[] {"world"}));
            }
        }

        [Test]
        public void ShouldFormat()
        {
            // given
            var builder = new SutBuilder();

            // sut            
            var sut = builder.Build();

            // when
            var result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello world!"));
        }
        
        [Test]
        public void ShouldFormat2()
        {
            // given
            var argProvider = Substitute.For<IArgumentsProvider>();
            argProvider.GetArguments().Returns(new object[] {"guys"});

            var somethingMaker = new SomethingMaker();
            var parametrizedSomethingMaker = new ParametrizedSomethingMaker(256);
            
            var builder = new SutBuilder(somethingMaker, parametrizedSomethingMaker)             
                .Inject(argProvider)
                .Configure<IFormatProvider>(fp => fp.GetFormat().Returns("Goodbye {0}!"));

            var logger = builder.Get<IMyLogger>();
            
            // sut            
            var sut = builder.Build();

            // when
            var result = sut.FormatMessage();

            // then
            logger.Received().Log("formatting ...");
            
            Assert.That(result, Is.EqualTo("Goodbye guys!"));                        
        }
    }
}