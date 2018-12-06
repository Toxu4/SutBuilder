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
            public SutBuilder()
            : base(builder =>
            {
                builder.Inject(
                    new SomethingMaker(),
                    new ParametrizedSomethingMaker(256));
                
                builder.Configure<IFormatProvider>(fp => fp.GetFormat().Returns("Hello {0}!"));
                builder.Configure<IArgumentsProvider>(ap => ap.GetArguments().Returns(new object[] {"world"}));                
            })
            {
            }
        }

        [Test]
        public void Should_Format_Using_Default_Config()
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
        public void Should_Format_Using_Additional_Injections_And_Configuration()
        {
            // given
            var argProvider = Substitute.For<IArgumentsProvider>();
            argProvider.GetArguments().Returns(new object[] {"guys"});

            var somethingMaker = new SomethingMaker();
            var parametrizedSomethingMaker = new ParametrizedSomethingMaker(256);
            
            var builder = new SutBuilder()
                .Inject(
                    somethingMaker, 
                    parametrizedSomethingMaker, 
                    argProvider)
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
        
        [Test]
        public void Should_Format_Using_Default_Config_After_Reset()
        { 
            /*
             * "Hello guys" case
             */
            
            // given
            var builder = new SutBuilder();

            builder.Configure<IArgumentsProvider>(ap => ap
                .GetArguments()
                .Returns(new object[] {"guys"}));

            // sut            
            var sut = builder.Build();

            // when 
            var result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello guys!"));

            
            /*
             * "Hello world" case
             */ 
            
            // given
            builder.Reset();
            
            // sut
            sut = builder.Build();
            
            // when
            result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello world!"));
            
            /*
             * "Hello dude" case
             */
            
            // given
            builder.Reset();

            builder.Configure<IArgumentsProvider>(ap => ap
                .GetArguments()
                .Returns(new object[] {"dude"}));
            
            // sut
            sut = builder.Build();
                        
            // when
            result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello dude!"));
        }
        
    }
}