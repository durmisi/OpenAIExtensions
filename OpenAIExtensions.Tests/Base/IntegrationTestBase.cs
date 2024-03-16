using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace OpenAIExtensions.Tests
{
    public abstract class IntegrationTestBase
    {
        private readonly ILoggerFactory _loggerFactory;

        protected IntegrationTestBase(ITestOutputHelper outputHelper)
        {

            Configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .AddEnvironmentVariables(prefix: "")
            .Build();

            _loggerFactory = LoggerFactory
                .Create(x => x.AddConsole());
        }

        public IConfigurationRoot Configuration { get; }

        protected ILogger<T> CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }

    }
}
