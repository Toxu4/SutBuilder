# SutBuilder [![Build status](https://ci.appveyor.com/api/projects/status/356qx7r7odkobkr9?svg=true)](https://ci.appveyor.com/project/Toxu4/sutbuilder) [![license](https://img.shields.io/github/license/mashape/apistatus.svg)]()


&nbsp; | `NuGet Package`
--- | ---
**SutBuilder** | [![nuget](https://img.shields.io/nuget/v/SutBuilder.svg)](https://www.nuget.org/packages/SutBuilder/)
**SutBuilder.NSubstitute** | [![nuget](https://img.shields.io/nuget/v/SutBuilder.NSubstitute.svg)](https://www.nuget.org/packages/SutBuilder.NSubstitute/)


# SutBuilder
System Under Test creation helper

Imagine that the Formatter class is our System Under Test

```CSharp
     public class Formatter
     {
         private readonly IFormatProvider _formatProvider;
         private readonly IArgumentsProvider _argumentsProvider;
         private readonly IMyLogger _logger;
         private readonly SomethingMaker _somethingMaker;
         private readonly ParametrizedSomethingMaker _parametrizedSomethingMaker;
         
 
         public Formatter(IFormatProvider formatProvider, IArgumentsProvider argumentsProvider, IMyLogger logger, SomethingMaker somethingMaker, ParametrizedSomethingMaker parametrizedSomethingMaker)
         {
             _formatProvider = formatProvider;
             _argumentsProvider = argumentsProvider;
             _logger = logger;
             _somethingMaker = somethingMaker;
             _parametrizedSomethingMaker = parametrizedSomethingMaker;
         }
 
         public string FormatMessage()
         {
             _somethingMaker.MakeSomething();
             _parametrizedSomethingMaker.MakeSomething();
             
             _logger.Log("formatting ...");
             
             return string.Format(_formatProvider.GetFormat(), _argumentsProvider.GetArguments());
         }
     }
     
    public interface IFormatProvider
    {
        string GetFormat();
    } 
    
    public interface IArgumentsProvider
    {
        object[] GetArguments();
    }
    
    public interface IMyLogger
    {
        void Log(string s);
    }
    
    public class SomethingMaker
    {
        public void MakeSomething()
        {
            // something
        }
    }
       
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
```

Test the Formatter class using NSubstituteSutBuilder

```CSharp
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
}```
