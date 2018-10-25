namespace SutBuilder.Tests.Unit
{
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
}