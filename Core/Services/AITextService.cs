using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace OpenAIExtensions.Services
{
    public interface IAITextService
    {
        public Task<string> Summarize(string input, CancellationToken ct = default);
        public Task<string> ChangeVoiceToEmotional(string input, CancellationToken ct = default);
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

        public async Task<string> ChangeVoiceToEmotional(string input, CancellationToken ct = default)
        {

            string prompt = $@"
                You are a helpful assistant.

        Title: Changing the Voice of Content to Emotionally Engaging

        Description: In this prompt, we aim to transform the voice of the provided content into an emotionally engaging style. Please generate text that conveys a range of emotions, such as joy, sadness, excitement, or empathy, to evoke specific feelings or reactions in the reader. Enhance the overall impact and resonance of the message by infusing the text with emotion.

        User Content: {input}

        Feel free to experiment with different approaches and techniques for infusing emotion into the provided content. Consider the potential implications and applications of emotionally engaging writing in various domains.
    ";

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage(prompt);

            var response = await chatCompletionService
                .GetChatMessageContentAsync(chatHistory: chatHistory,
                executionSettings: new OpenAIPromptExecutionSettings()
                {
                    MaxTokens = 1000,
                    Temperature = 0.7f,
                    FrequencyPenalty = 0.3f,
                    PresencePenalty = 0.3f,
                    TopP = 0.95f
                },
                kernel: _kernel,
                cancellationToken: ct);

            string responseMessage = response.Content ?? "";

            return responseMessage;

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

        public async Task<string> Summarize(string input, CancellationToken ct = default)
        {
            string prompt = $@"
                You are a helpful assistant.

                Title: Summarizing Text

                Description: In this prompt, we aim to generate a concise summary of the provided text. Summarization involves condensing the main points and key information of the text into a shorter form while retaining its essence and meaning.

                User Text: {input}

                Please generate a summary of the provided text. Focus on capturing the main ideas, key points, and important details while keeping the summary brief and easy to understand.

                Feel free to experiment with different summarization techniques and approaches to produce an accurate and effective summary that effectively conveys the essence of the original text.

                    ";

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var chatHistory = new ChatHistory();

            chatHistory.AddUserMessage(prompt);

            var executionSettings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 600,
                Temperature = 0.5f,
                FrequencyPenalty = 0.4f,
                PresencePenalty = 0.4f, 
                TopP = 0.9f
            };

            var response = await chatCompletionService
                .GetChatMessageContentAsync(chatHistory: chatHistory,
                executionSettings: executionSettings,
                kernel: _kernel,
                cancellationToken: ct);

            string responseMessage = response.Content ?? "";

            return responseMessage;
        }
    }
}
