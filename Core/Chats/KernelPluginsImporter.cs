using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using SemanticKernel.Connectors.Memory.SqlServer;

namespace OpenAIExtensions.Chats
{
    public static class KernelPluginsImporter
    {
        public static async void ImportTextMemoryPlugin(
            this Kernel kernel,
            string connectionString,
            string schema,
            CancellationToken ct = default)
        {
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
        }
    }
}