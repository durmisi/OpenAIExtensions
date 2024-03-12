using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Chats
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
            string? systemMessage = null,
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

            var history = new ChatHistory();

            if (!string.IsNullOrEmpty(systemMessage))
            {

                history.AddSystemMessage(systemMessage);
            }

            foreach (var item in chatHistory)
            {
                if (item.InnerContent != null && !item.InnerContent.Equals(systemMessage?.ToString()))
                {
                    continue;
                }

                history.Add(item);
            }

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            IAsyncEnumerable<StreamingChatMessageContent> result = chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory: history,
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
