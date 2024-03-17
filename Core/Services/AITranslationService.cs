using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAIExtensions.Services
{
    public interface ITranslationService
    {
        Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage = "English");

        Task<Dictionary<string, string>?> TranslateToManyAsync(string text, string fromLanguage, string[]? toLanguages = null);
    }

    public class AITranslationService : ITranslationService
    {
        private readonly OpenAIClient _client;

        private readonly ILogger<AITranslationService> _logger;

        private readonly string _deploymentName = "gpt-35-turbo-0613";

        public AITranslationService(
            IAIBroker aIBroker,
            ILogger<AITranslationService> logger, string? deploymentName = null)
        {
            _logger = logger;
            _client = aIBroker.GetClient();

            if (!string.IsNullOrEmpty(deploymentName))
            {
                _deploymentName = deploymentName;
            }
        }

        private ChatCompletionsOptions GetCompletionsOptions()
        {
            var completionOptions = new ChatCompletionsOptions
            {
                DeploymentName = _deploymentName,
                MaxTokens = 60,
                Temperature = 0f,
                FrequencyPenalty = 0.0f,
                PresencePenalty = 0.0f,
                NucleusSamplingFactor = 1 // Top P
            };

            return completionOptions;
        }

        public async Task<string?> TranslateAsync(string text, string fromLanguage, string toLanguage = "English")
        {
            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguage}.";

            var userPrompt = $@"Translate this into 1. {toLanguage}.
                    {text}";

            var completionOptions = GetCompletionsOptions();

            completionOptions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
            completionOptions.Messages.Add(new ChatRequestUserMessage(userPrompt));

            ChatCompletions response = await _client.GetChatCompletionsAsync(completionOptions);

            var choice = response.Choices.FirstOrDefault();

            if (choice != null)
            {
                return choice?.Message?.Content;
            }

            return null;
        }

        public async Task<Dictionary<string, string>?> TranslateToManyAsync(string text, string fromLanguage, string[]? toLanguages = null)
        {
            if (toLanguages == null)
            {
                toLanguages = ["English"];
            }

            var toLanguagesString = "";

            for (int i = 0; i < toLanguages.Length; i++)
            {
                string? language = toLanguages[i];
                toLanguagesString += $" {i + 1}.{language}, ";
            }

            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguagesString}.";

            var userPrompt = $@"Translate this into {toLanguagesString} {text} and return the result as json dictionary of language/translation but exclude the original language.";

            var completionOptions = GetCompletionsOptions();

            completionOptions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
            completionOptions.Messages.Add(new ChatRequestUserMessage(userPrompt));

            ChatCompletions response = await _client.GetChatCompletionsAsync(completionOptions);

            var choice = response.Choices.FirstOrDefault();

            if (choice != null)
            {
                var serializedOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.WriteAsString,
                };

                using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(choice.Message.Content));
                var result = JsonSerializer.Deserialize<Dictionary<string, string>?>(stream, serializedOptions);

                return result;
            }

            return null;
        }
    }
}