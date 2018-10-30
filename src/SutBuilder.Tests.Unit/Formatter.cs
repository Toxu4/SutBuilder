namespace SutBuilder.Tests.Unit
 {
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
 }