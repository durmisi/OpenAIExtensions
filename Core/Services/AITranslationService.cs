using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAIExtensions.Services
{
    public interface ITranslationService
    {
        Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage = "English", CancellationToken ct = default);

        Task<Dictionary<string, string>?> TranslateToManyAsync(string text, string fromLanguage, string[]? toLanguages = null, CancellationToken ct = default);
    }

    public class AITranslationService : ITranslationService
    {
        private readonly Kernel _kernel;
        private readonly ILogger<AITranslationService> _logger;

        public AITranslationService(
            Kernel kernel,
            ILogger<AITranslationService> logger)
        {
            _kernel = kernel;
            _logger = logger;
        }

        public async Task<string?> TranslateAsync(string text,
            string fromLanguage,
            string toLanguage = "English",
            CancellationToken ct = default)
        {
            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguage}.";

            var userPrompt = $@"Translate this into 1. {toLanguage}.
                    {text}";

            var chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage(systemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await GetResponse(chatHistory, ct);

            string responseMessage = response.Content ?? "";

            if (!string.IsNullOrEmpty(responseMessage))
            {
                return responseMessage;
            }

            return null;
        }

        private async Task<ChatMessageContent> GetResponse(ChatHistory chatHistory,
            CancellationToken ct)
        {
            var openAIPromptExecutionSettings = GetExecutionSettings();

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var response = await chatCompletionService
                .GetChatMessageContentAsync(chatHistory: chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: _kernel,
                cancellationToken: ct);

            return response;
        }

        private static PromptExecutionSettings GetExecutionSettings() => new OpenAIPromptExecutionSettings()
        {
            MaxTokens = 60,
            Temperature = 0f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f,
            TopP = 1 // Top P
        };

        public async Task<Dictionary<string, string>?> TranslateToManyAsync(string text,
            string fromLanguage,
            string[]? toLanguages = null,
            CancellationToken ct = default)
        {
            toLanguages ??= ["English"];

            var toLanguagesString = "";

            for (int i = 0; i < toLanguages.Length; i++)
            {
                string? language = toLanguages[i];
                toLanguagesString += $" {i + 1}.{language}, ";
            }

            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguagesString}.";

            var userPrompt = $@"Translate this into {toLanguagesString} {text} and return the result as json dictionary of language/translation but exclude the original language.";

            var chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage(systemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await GetResponse(chatHistory, ct);

            string responseMessage = response.Content ?? "";

            if (!string.IsNullOrEmpty(responseMessage))
            {
                JsonSerializerOptions serializedOptions = new()
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.WriteAsString,
                };

                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseMessage));
                var result = JsonSerializer.Deserialize<Dictionary<string, string>?>(stream, serializedOptions);

                return result;
            }

            return null;
        }
    }
}