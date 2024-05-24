using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Services
{
    public interface IAITextService
    {
        public Task<string> Brainstorm(string input, CancellationToken ct = default);
    }

    public class AITextService : IAITextService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AITextService> _logger;

        public AITextService(
            Kernel kernel,
            ILogger<AITextService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<string> Brainstorm(string input, CancellationToken ct = default)
        {

            string prompt = $@"
                You are a helpful assistant.
                I have an idea I'd like to brainstorm around.
    
                Here it is: {input}
    
                Can you provide various aspects such as potential challenges, opportunities, innovative angles, and more? I'm looking for detailed suggestions from different perspectives.
            ";

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage(prompt);

            var response = await chatCompletionService
                .GetChatMessageContentAsync(chatHistory: chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = 1000,
                    Temperature = 0.5f,
                    FrequencyPenalty = 0.5f,
                    PresencePenalty = 0.5f,
                    TopP = 0.9f // Top P
                },
                kernel: _kernel,
                cancellationToken: ct);

            string responseMessage = response.Content ?? "";

            return responseMessage;
        }

    }
}
