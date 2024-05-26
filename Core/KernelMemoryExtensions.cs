using KernelMemory.MemoryStorage.SqlServer;
using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.KernelMemory.AI.AzureOpenAI;
using Microsoft.KernelMemory.AI.OpenAI;

namespace OpenAIExtensions
{
    public class CreateKernelMemoryRequest
    {
        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
        public string ConnectionString { get; set; }
        public string? Schema { get; set; }
    }

    public static class KernelMemoryExtensions
    {
        public static IKernelMemory WithSqlServerMemoryDb(CreateKernelMemoryRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var kernelMemoryBuilder = new KernelMemoryBuilder()
                .WithAzureOpenAIDefaults(
                request.Endpoint,
                request.ApiKey);

            var config = new SqlServerConfig()
            {
                ConnectionString = request.ConnectionString,
                Schema = request.Schema ?? "ai"
            };

            var kernelMemory = kernelMemoryBuilder
                .WithSqlServerMemoryDb(config)
                .Build<MemoryServerless>();

            return kernelMemory!;
        }


        public static IKernelMemory WithSimpleVectorDb(string endpoint, string apiKey)
        {
            var kernelMemoryBuilder = new KernelMemoryBuilder()
               .WithAzureOpenAIDefaults(
               endpoint,
               apiKey);

            var kernelMemory = kernelMemoryBuilder
                .WithSimpleVectorDb(new Microsoft.KernelMemory.MemoryStorage.DevTools.SimpleVectorDbConfig()
                {
                    StorageType = Microsoft.KernelMemory.FileSystem.DevTools.FileSystemTypes.Volatile
                })
                .Build<MemoryServerless>();

            return kernelMemory!;
        }

        public static IKernelMemoryBuilder WithAzureOpenAIDefaults(
            this IKernelMemoryBuilder kernelMemoryBuilder,
            string endpoint,
            string apiKey,
            ITextTokenizer? textGenerationTokenizer = null,
            ITextTokenizer? textEmbeddingTokenizer = null,
            ILoggerFactory? loggerFactory = null,
            bool onlyForRetrieval = false,
            HttpClient? httpClient = null)
        {
            textGenerationTokenizer ??= new DefaultGPTTokenizer();
            textEmbeddingTokenizer ??= new DefaultGPTTokenizer();

            var textEmbbedingAIConfig = new AzureOpenAIConfig
            {
                APIKey = apiKey,
                Endpoint = endpoint,
                Deployment = "text-embedding-ada-002",
                MaxRetries = 3,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                MaxTokenTotal = 8191,
            };

            var textGenerationAIConfig = new AzureOpenAIConfig
            {
                APIKey = apiKey,
                Endpoint = endpoint,
                Deployment = "gpt-35-turbo-0613",
                MaxRetries = 3,
                Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                MaxTokenTotal = 16384,
            };

            textEmbbedingAIConfig.Validate();
            textGenerationAIConfig.Validate();

            //ITextEmbeddingGeneration
            kernelMemoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
                textEmbbedingAIConfig,
                textEmbeddingTokenizer,
                onlyForRetrieval: onlyForRetrieval,
                httpClient: httpClient);

            //ITextGeneration
            kernelMemoryBuilder.WithAzureOpenAITextGeneration(
                textGenerationAIConfig,
                textGenerationTokenizer,
                httpClient);

            return kernelMemoryBuilder;
        }
    }
}