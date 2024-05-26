using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Managers
{
    public interface IAIConversationManager
    {
        ValueTask<string?> ProcessConversationAsync(
            ChatHistory chatHistory,
            string? systemMessage = null,
            OpenAIPromptExecutionSettings? executionSettings = null,
            CancellationToken ct = default);
    }

    public class AIConversationManager : IAIConversationManager
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AIConversationManager> _logger;

        public AIConversationManager(Kernel kernel,
            ILogger<AIConversationManager> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public IAsyncEnumerable<StreamingChatMessageContent>? ProcessConversationStreamAsync(
            ChatHistory chatHistory,
            string? systemPropmpt = null,
            OpenAIPromptExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(chatHistory);

            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };

            if (executionSettings != null)
            {
                openAIPromptExecutionSettings = executionSettings;
            }

            if (string.IsNullOrEmpty(openAIPromptExecutionSettings.ChatSystemPrompt)
                && !string.IsNullOrEmpty(systemPropmpt))
            {
                openAIPromptExecutionSettings.ChatSystemPrompt = systemPropmpt;
            }

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService
                .GetStreamingChatMessageContentsAsync(chatHistory: chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: _kernel,
                cancellationToken: ct);

            return result;
        }

        public async ValueTask<string?> ProcessConversationAsync(
            ChatHistory chatHistory,
            string? systemMessage = null,
            OpenAIPromptExecutionSettings? executionSettings = null,
            CancellationToken ct = default)
        {
            var result = ProcessConversationStreamAsync(chatHistory, systemMessage, executionSettings, ct);

            if (result != null)
            {
                string fullMessage = "";
                await foreach (var content in result)
                {
                    fullMessage += content.Content;
                }

                return fullMessage;
            }

            return null;
        }
    }
}