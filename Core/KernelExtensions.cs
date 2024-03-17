using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using SemanticKernel.Connectors.Memory.SqlServer;

namespace OpenAIExtensions
{
    public static class KernelExtensions
    {
        public static async Task<IKernelMemory> AddSqlServerKernelMemory(
            this Kernel kernel,
            string endpoint,
            string apiKey,
            string connectionString,
            string? schema = null,
            CancellationToken ct = default)
        {
            var kernelMemory = KernelMemoryFactory.WithSqlServerMemoryDb(
                new CreateKernelMemoryRequest()
                {
                    Endpoint = endpoint,
                    ApiKey = apiKey,
                    ConnectionString = connectionString,
                    Schema = schema ?? "ai"
                });

            //TextMemoryPlugin
            var config = new SqlServerConfig()
            {
                ConnectionString = connectionString,
                Schema = schema ?? "ai"
            };

            var sqlMemoryStore = await SqlServerMemoryStore
                .ConnectAsync(connectionString: connectionString, config, cancellationToken: ct);

            var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

            var semanticTextMemory = new SemanticTextMemory(sqlMemoryStore, embeddingGenerator);
            kernel.ImportPluginFromObject(new TextMemoryPlugin(semanticTextMemory));

            //MemoryPlugin
            kernel.ImportPluginFromObject(new MemoryPlugin(kernelMemory, waitForIngestionToComplete: true), "memory");

            return kernelMemory;
        }
    }
}