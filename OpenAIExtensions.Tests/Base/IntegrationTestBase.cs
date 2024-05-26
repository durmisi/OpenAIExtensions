using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests.Base
{
    public abstract class IntegrationTestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITestOutputHelper _outputHelper;

        protected IntegrationTestBase(ITestOutputHelper outputHelper)
        {
            Configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .AddEnvironmentVariables(prefix: "")
            .Build();

            _loggerFactory = LoggerFactory
                .Create(x => x.AddConsole());

            _outputHelper = outputHelper;
        }

        public IConfigurationRoot Configuration { get; }

        protected ILogger<T> CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

        protected void WriteToConsole(string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _outputHelper.WriteLine(message);
            }
        }
    }
}