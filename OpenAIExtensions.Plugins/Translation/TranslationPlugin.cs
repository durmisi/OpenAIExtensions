using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAIExtensions.Plugins.Translation
{

    /// <summary>
    /// TODO: https://github.com/microsoft/semantic-kernel/issues/6281
    /// </summary>
    public class TranslationPlugin
    {
        [KernelFunction, Description("Translate a text from one language to another")]

        public async Task<string?> TranslateAsync(
            Kernel kernel,
            [Description("The text to translate")] string text,
            [Description("The source language of the text")] string fromLanguage,
            [Description("The target language to translate the text into. Default is \"English\"")] string toLanguage = "English", CancellationToken ct = default)
        {
            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguage}.";
            var userPrompt = $@"Translate this into 1. {toLanguage}. {text}";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await GetResponse(kernel, chatHistory, ct);

            string responseMessage = response.Content ?? "";

            if (!string.IsNullOrEmpty(responseMessage))
            {
                return responseMessage;
            }

            return null;
        }

        [KernelFunction, Description("Translate a text from one language to multiple target languages")]
        public async Task<Dictionary<string, string>?> TranslateToManyAsync(
            Kernel kernel,
            [Description("The text to translate")] string text,
            [Description("The source language of the text.")] string fromLanguage,
            [Description("An array of target languages to translate the text into. Defaults to \"English\" if not provided.")] string[]? toLanguages = null,
            CancellationToken ct = default)
        {
            toLanguages ??= new string[] { "English" };

            var toLanguagesString = string.Join(", ", toLanguages.Select((lang, index) => $"{index + 1}.{lang}"));

            var systemPrompt = $"You are a virtual agent that helps users translate passages from {fromLanguage} to {toLanguagesString}.";
            var userPrompt = $@"Translate this into {toLanguagesString} {text} and return the result as a json dictionary of language/translation but exclude the original language.";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(systemPrompt);
            chatHistory.AddUserMessage(userPrompt);

            var response = await GetResponse(kernel, chatHistory, ct);
            string responseMessage = response.Content ?? "";

            if (!string.IsNullOrEmpty(responseMessage))
            {
                var serializedOptions = new JsonSerializerOptions
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

        private async Task<ChatMessageContent> GetResponse(

            Kernel kernel,
            ChatHistory chatHistory, CancellationToken ct)
        {
            var openAIPromptExecutionSettings = GetExecutionSettings();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var response = await chatCompletionService
                .GetChatMessageContentAsync(chatHistory: chatHistory,
                                            executionSettings: openAIPromptExecutionSettings,
                                            kernel: kernel,
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
    }
}
