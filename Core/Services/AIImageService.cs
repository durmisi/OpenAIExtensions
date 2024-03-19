using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace OpenAIExtensions.Services
{
    public interface IAIImageService
    {
        Task<string?> DescribeAsync(string rawImageUrl, CancellationToken ct = default);
    }

    /// <summary>
    /// https://drlee.io/transforming-audio-to-text-with-openais-speech-to-text-api-a-practical-step-by-step-guide-8139e4e65fdf
    /// </summary>
    public class AIImageService : IAIImageService
    {
        private readonly Kernel _kernel;

        private readonly ILogger<AIImageService> _logger;

        public AIImageService(
            Kernel kernel,
            ILogger<AIImageService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<string?> DescribeAsync(string rawImageUrl, CancellationToken ct = default)
        {
            var chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage("You are a friendly assistant that helps decribe images.");

            chatHistory.AddUserMessage(new ChatMessageContentItemCollection
            {
                new TextContent("What is this image?"),
                new ImageContent(new Uri(rawImageUrl))
            });

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();   
            var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: ct);

            if (!string.IsNullOrEmpty(reply.Content))
            {
                return reply.Content;
            }

            return null;
        }
    }
}