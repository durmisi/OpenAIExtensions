using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;

namespace OpenAIExtensions.Services
{
    public interface IAIImageService
    {
        Task<string?> DescribeAsync(string rawImageUrl);
    }

    /// <summary>
    /// https://drlee.io/transforming-audio-to-text-with-openais-speech-to-text-api-a-practical-step-by-step-guide-8139e4e65fdf
    /// </summary>
    public class AIImageService : IAIImageService
    {
        private readonly OpenAIClient _client;

        private readonly ILogger<AIImageService> _logger;

        private readonly string _deploymentName = "gpt-4-vision-preview";

        public AIImageService(
            IAIBroker aIBroker,
            ILogger<AIImageService> logger, string? deploymentName = null)
        {
            _logger = logger;
            _client = aIBroker.GetClient();

            if (!string.IsNullOrEmpty(deploymentName))
            {
                _deploymentName = deploymentName;
            }
        }

        public async Task<string?> DescribeAsync(string rawImageUrl)
        {
            ChatCompletionsOptions chatCompletionsOptions = new()
            {
                DeploymentName = _deploymentName,
                Messages =
                {
                    new ChatRequestSystemMessage("You are a helpful assistant that describes images."),
                    new ChatRequestUserMessage(
                        new ChatMessageTextContentItem("Hi! Please describe this image"),
                        new ChatMessageImageContentItem(new Uri(rawImageUrl))),
                },
            };

            var chatResponse = await _client.GetChatCompletionsAsync(chatCompletionsOptions);

            var choice = chatResponse.Value.Choices.FirstOrDefault();
            if (choice?.FinishDetails is StopFinishDetails || choice?.FinishReason == CompletionsFinishReason.Stopped)
            {
                return choice.Message.Content;
            }

            if (!string.IsNullOrEmpty(choice?.Message?.Content))
            {
                return choice.Message.Content;
            }

            return null;
        }

    }
}
