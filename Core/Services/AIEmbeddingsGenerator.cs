using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace OpenAIExtensions.Services
{
    public interface IAIEmbeddingsGenerator
    {
        Task<IList<ReadOnlyMemory<float>>> GetEmbeddingsAsync(string input);
    }

    public class AIEmbeddingsGenerator : IAIEmbeddingsGenerator
    {
        private readonly ILogger<AIEmbeddingsGenerator> _logger;

        private readonly string _deploymentName = "text-embedding-ada-002";

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        public AIEmbeddingsGenerator(
            string endpoint,
            string apiKey,
            ILogger<AIEmbeddingsGenerator> logger,
            string? deploymentName = null)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException($"'{nameof(endpoint)}' cannot be null or empty.", nameof(endpoint));
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or empty.", nameof(apiKey));
            }

            ArgumentNullException.ThrowIfNull(logger);

            _deploymentName = !string.IsNullOrEmpty(deploymentName)
                ? deploymentName
                : _deploymentName;

            var kernel = SematicKernelBuilder.Create()
                .AddAITextEmbeddingGeneration(endpoint, apiKey, _deploymentName)
                .Build();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            _logger = logger;
        }

        public AIEmbeddingsGenerator(
            Kernel kernel,
            ILogger<AIEmbeddingsGenerator> logger)
        {
            ArgumentNullException.ThrowIfNull(kernel);
            ArgumentNullException.ThrowIfNull(logger);

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            _logger = logger;
        }

        public async Task<IList<ReadOnlyMemory<float>>> GetEmbeddingsAsync(string input)
        {
            var response = await _textEmbeddingGenerationService
                .GenerateEmbeddingsAsync([input]);

            return response;
        }
    }
}