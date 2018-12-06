using NSubstitute;
using NUnit.Framework;

namespace SutBuilder.Tests.Unit
{
    public class AlternativeFormatterTests : NSubstituteTestFixtureBase<Formatter>
    {
        public AlternativeFormatterTests()
            :base(builder =>
            {
                builder.Inject(
                    new SomethingMaker(), 
                    new ParametrizedSomethingMaker(100));
                
                builder.Configure<IFormatProvider>(fp => fp.GetFormat().Returns("Hello {0}!"));
                builder.Configure<IArgumentsProvider>(ap => ap.GetArguments().Returns(new object[] {"world"}));                
            })
        {            
        }
        
        [Test]
        public void Should_Format_Using_Default_Config()
        {
            // given

            // sut            
            var sut = SutBuilder.Build();

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
            
            SutBuilder
                .Inject(
                    somethingMaker, 
                    parametrizedSomethingMaker, 
                    argProvider)
                .Configure<IFormatProvider>(fp => fp.GetFormat().Returns("Goodbye {0}!"));

            var logger = SutBuilder.Get<IMyLogger>();
            
            // sut            
            var sut = SutBuilder.Build();

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
            SutBuilder.Configure<IArgumentsProvider>(ap => ap
                .GetArguments()
                .Returns(new object[] {"guys"}));

            // sut            
            var sut = SutBuilder.Build();

            // when 
            var result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello guys!"));

            
            /*
             * "Hello world" case
             */ 
            
            // given
            SutBuilder.Reset();
            
            // sut
            sut = SutBuilder.Build();
            
            // when
            result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello world!"));
            
            /*
             * "Hello dude" case
             */
            
            // given
            SutBuilder.Reset();

            SutBuilder.Configure<IArgumentsProvider>(ap => ap
                .GetArguments()
                .Returns(new object[] {"dude"}));
            
            // sut
            sut = SutBuilder.Build();
                        
            // when
            result = sut.FormatMessage();

            // then
            Assert.That(result, Is.EqualTo("Hello dude!"));
        }
        
        [Test]
        public void Should_Be_Able_To_Use_Descendant_Classes()
        {
            // given

            // when

            // then
            Assert.DoesNotThrow(() => SutBuilder.Inject(new SomethingElseMaker()));            
            Assert.DoesNotThrow(() => SutBuilder.Inject(Substitute.For<SomethingElseMaker>()));            
        }

    }
}