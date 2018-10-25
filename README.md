# SutBuilder [![Build status](https://ci.appveyor.com/api/projects/status/356qx7r7odkobkr9?svg=true)](https://ci.appveyor.com/project/Toxu4/sutbuilder) [![license](https://img.shields.io/github/license/mashape/apistatus.svg)]()


&nbsp; | `NuGet Package`
--- | ---
**SutBuilder** | [![NuGet Version](https://buildstats.info/nuget/SutBuilder)](https://www.nuget.org/packages/SutBuilder) 
**SutBuilder.NSubstitute** | [![NuGet Version](https://buildstats.info/nuget/SutBuilder.NSubstitute)](https://www.nuget.org/packages/SutBuilder.NSubstitute) 


# SutBuilder
System Under Test creation helper

Imagine that the Formatter class is our System Under Test

```CSharp
     public class Formatter
     {
         private readonly IFormatProvider _formatProvider;
         private readonly IArgumentsProvider _argumentsProvider;
         private readonly IMyLogger _logger;
 
         public Formatter(IFormatProvider formatProvider, IArgumentsProvider argumentsProvider, IMyLogger logger)
         {
             _formatProvider = formatProvider;
             _argumentsProvider = argumentsProvider;
             _logger = logger;
         }
 
         public string FormatMessage()
         {
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
```

Test the Formatter class using NSubstituteSutBuilder

```CSharp
 [TestFixture]
    public class FormatterTests
    {
        private class SutBuilder : NSubstituteSutBuilder<Formatter>
        {
            public SutBuilder()
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

            var builder = new SutBuilder()             
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
```
