using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;

namespace OpenAIExtensions.Plugins.Cognitive
{

    /// <summary>
    /// https://drlee.io/transforming-audio-to-text-with-openais-speech-to-text-api-a-practical-step-by-step-guide-8139e4e65fdf
    /// </summary>
    public class CognitivePlugin
    {

        [KernelFunction, Description("Describe the contents of an image")]
        public async Task<string?> DescribeAsync(
            Kernel kernel,
           [Description("The URL of the image to describe.")] string imageUrl,
           CancellationToken ct = default)
        {
            var chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage("You are a friendly assistant that helps decribe images.");

            chatHistory.AddUserMessage(new ChatMessageContentItemCollection
            {
                new TextContent("What is this image?"),
                new ImageContent(new Uri(imageUrl))
            });

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var reply = await chatCompletionService.GetChatMessageContentAsync(chatHistory, cancellationToken: ct);

            if (!string.IsNullOrEmpty(reply.Content))
            {
                return reply.Content;
            }

            return null;
        }
    }
}